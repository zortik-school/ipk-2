using System.Net;
using System.Net.Sockets;
using IPK_2.Message;

namespace IPK_2.Client;

public class Udp(string ip, int port) : SocketClient
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private UdpClient? _client;
    private IPEndPoint? _remoteEndPoint;

    public int Retransmissions { get; set; } = 3;
    public int Timeout { get; set; } = 250;
    
    private readonly HashSet<ushort> _processedMessageIds = new();
    private readonly HashSet<ushort> _processedMessageIdsLocally = new();
    
    private ushort _messageId;

    public override void Start()
    {
        IPAddress ipv4Address = Dns.GetHostAddresses(ip)
            .First(a => a.AddressFamily == AddressFamily.InterNetwork);

        _remoteEndPoint = new IPEndPoint(ipv4Address, port);
        
        //_client = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        _client = new UdpClient(4567);
        _messageId = 0;
        
        Console.Error.WriteLine($"Resolved IP: {_remoteEndPoint?.Address}");
        Console.Error.WriteLine($"Listening on: {_client.Client.LocalEndPoint}");
        Console.Error.WriteLine("UDP client started.");

        CancellationToken cancellationToken = _cancellationTokenSource.Token;
        
        try
        {
            Task.WhenAll(
                StartSender(cancellationToken),
                StartReceiver(cancellationToken)).Wait(cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private byte[] GenerateMessageId()
    {
        ushort messageIdToSend = _messageId++;

        return BitConverter.GetBytes(messageIdToSend);
    }

    private async Task SendMessage(IMessage message)
    {
        byte[] messageId = GenerateMessageId();
        byte[] data = message.ToUdp(messageId);
        
        await SendUdpMessage(data, messageId);
    }

    private async Task SendUdpMessage(byte[] data, byte[] messageId)
    {
        await _client!.SendAsync(data, data.Length, _remoteEndPoint);
        
        Console.Error.WriteLine($"Sent message with ID: {BitConverter.ToUInt16(messageId, 0):X4}");
    }

    private async Task SendMessageWithConfirmation(IMessage message, CancellationToken cancellationToken)
    {
        byte[] messageId = GenerateMessageId();
        byte[] data = message.ToUdp(messageId);

        int retryCount = 0;
        bool receivedConfirmation = false;

        while (retryCount < Retransmissions && !receivedConfirmation && !cancellationToken.IsCancellationRequested)
        {
            await SendUdpMessage(data, messageId);

            receivedConfirmation = await WaitForConfirmation(messageId, cancellationToken);
            
            if (!receivedConfirmation)
            {
                Console.Error.WriteLine("No confirmation received, retrying...");
                retryCount++;
                await Task.Delay(Timeout);
            }
        }

        if (!receivedConfirmation)
        {
            Console.Error.WriteLine("Failed to receive confirmation after retries.");
            Console.Error.WriteLine($"Processed IDs: {_processedMessageIds}");
        }
    }

    private async Task<bool> WaitForConfirmation(byte[] messageId, CancellationToken cancellationToken)
    {
        DateTime startTime = DateTime.Now;
        while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(Timeout))
        {
            if (_processedMessageIds.Contains(BitConverter.ToUInt16(messageId, 0)))
            {
                return true;
            }

            await Task.Delay(10, cancellationToken);
        }
        return false;
    }

    private async Task<List<IMessage>> HandleUdpResponse(List<IMessage> messages, CancellationToken cancellationToken)
    {
        List<IMessage> toSend = [];
        
        foreach (var message in messages)
        {
            if (message is ConfirmMessage)
            {
                Console.Error.WriteLine("Received confirmation.");
                
                if (message.RefMessageId != null)
                {
                    ushort refMessageId = (ushort) message.RefMessageId;
                
                    if (_processedMessageIds.Contains(refMessageId))
                    {
                        Console.Error.WriteLine($"Duplicate message with ID: {refMessageId:X4}, ignoring.");
                    }
                    else
                    {
                        _processedMessageIds.Add(refMessageId!);
                        //Console.Error.WriteLine($"Processed message with ID: {refMessageId:X4}");
                    }
                }
                
                continue;
            }

            if (message.MessageId != null)
            {
                ushort messageId = (ushort) message.MessageId;

                if (message.ExpectsConfirmation())
                {
                    SendConfirmation(messageId);
                }
                
                if (_processedMessageIdsLocally.Contains(messageId))
                {
                    Console.Error.WriteLine($"Duplicate message with ID: {messageId:X4}, ignoring.");
                    continue;
                }
                else
                {
                    _processedMessageIdsLocally.Add(messageId);
                    
                    List<IMessage> toSend1 = await base.HandleResponse([message], cancellationToken);
                    
                    toSend.AddRange(toSend1);
                    
                    //Console.Error.WriteLine($"Processed message with ID: {messageId:X4}");
                }
            }
        }

        return toSend;
    }

    private void SendConfirmation(ushort messageId)
    {
        var confirmMessage = new ConfirmMessage(RefMessageId: messageId);
        byte[] confirmData = confirmMessage.ToUdp(GenerateMessageId());

        _client!.SendAsync(confirmData, confirmData.Length, _remoteEndPoint);
        
        //Console.Error.WriteLine($"Sent confirmation for message ID: {messageId:X4}");
    }

    private async Task StartSender(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            string? message = await Task.Run(Console.ReadLine, cancellationToken);
            
            if (message == null)
            {
                continue;
            }

            string[] args = message.Split(" ");

            _ = Task.Run(async () => await InterceptInputAndSend(args, cancellationToken), cancellationToken);
        }
    }

    private async Task InterceptInputAndSend(string[] args, CancellationToken cancellationToken)
    {
        List<IMessage> messagesToSend = await InterceptInput(args, cancellationToken);
                
        foreach (IMessage messageToSend in messagesToSend)
        {
            _ = Task.Run(async () =>
            {
                if (messageToSend.ExpectsConfirmation())
                {
                    await SendMessageWithConfirmation(messageToSend, cancellationToken);
                }
                else
                {
                    await SendMessage(messageToSend);
                }
            }, cancellationToken);
        }
    }

    private async Task StartReceiver(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                UdpReceiveResult result = await _client!.ReceiveAsync(cancellationToken);
                
                IPEndPoint remoteEndPoint = result.RemoteEndPoint;
                byte[] data = result.Buffer;
                
                //Console.Error.WriteLine($"Received data from {remoteEndPoint}: {BitConverter.ToString(data)}");

                _remoteEndPoint = remoteEndPoint;

                List<IMessage> messages = IMessage.ParseUdp(data);

                _ = Task.Run(async () =>
                {
                    try
                    {
                        List<IMessage> messagesToSend = await HandleUdpResponse(messages, cancellationToken);
                    
                        foreach (IMessage messageToSend in messagesToSend)
                        {
                            await SendMessage(messageToSend);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e);
                    }
                }, cancellationToken);
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.TimedOut)
                {
                    continue;
                }

                Console.Error.WriteLine($"Error while receiving data: {e.Message}");
            }
        }
    }

    public override void Stop()
    {
        _cancellationTokenSource.Cancel();
        _client?.Close();
    }
}

