using System.Net;
using System.Net.Sockets;
using IPK_2.Message;

namespace IPK_2.Client;

public class UdpClientClass(string ip, int port) : SocketClient
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly IPEndPoint _serverEndpoint = new(IPAddress.Parse(ip), port);
    private UdpClient? _client;

    public override void Start()
    {
        _client = new UdpClient();
        Console.WriteLine("UDP klient spuštěn.");

        CancellationToken cancellationToken = _cancellationTokenSource.Token;

        Parallel.Invoke(
            () => StartSender(cancellationToken),
            () => StartReceiver(cancellationToken)
        );
    }

    private async Task SendMessage(IMessage message, CancellationToken cancellationToken)
    {
        byte[] messageId = []; // TODO
        
        byte[] data = message.ToUdp(messageId);
        
        await _client!.SendAsync(data, data.Length, _serverEndpoint);
        
        // TODO
    }

    private async Task InterceptInputAndSend(string[] args, CancellationToken cancellationToken)
    {
        List<IMessage> messagesToSend = await InterceptInput(args, cancellationToken);
                
        foreach (IMessage messageToSend in messagesToSend)
        {
            Task.Run(async () => await SendMessage(messageToSend, cancellationToken), cancellationToken);
        }
    }

    private void StartSender(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            string? message = Console.ReadLine();
            if (message == null)
            {
                continue;
            }

            string[] args = message.Split(" ");

            Task.Run(async () => await InterceptInputAndSend(args, cancellationToken), cancellationToken);
        }
    }

    private void StartReceiver(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                IPEndPoint? remoteEndPoint = null;
                
                byte[] data = _client!.Receive(ref remoteEndPoint);
                
                List<IMessage> messages = IMessage.ParseUdp(data);

                Task.Run(() => HandleResponse(messages, cancellationToken), cancellationToken);
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.TimedOut)
                {
                    continue;
                }

                Console.WriteLine($"Chyba při příjmu dat: {e.Message}");
            }
        }
    }

    public override void Stop()
    {
        Console.WriteLine("Stopping...");
        
        _cancellationTokenSource.Cancel();
        _client?.Close();
    }
}

