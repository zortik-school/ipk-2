using System.Net.Sockets;
using System.Text;
using IPK_2.Message;

namespace IPK_2.Client;

public class Tcp(string ip, int port) : SocketClient
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private NetworkStream? _stream;
    
    public override void Start()
    {
        TcpClient client = new TcpClient(ip, port);
        _stream = client.GetStream();
        
        Console.Error.WriteLine("Connected to server.");
        
        CancellationToken cancellationToken = _cancellationTokenSource.Token;

        try
        {
            Task.WhenAll(
                StartSender(cancellationToken),
                StartReceiver(_stream, cancellationToken)).Wait(cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task SendMessage(IMessage message, CancellationToken cancellationToken)
    {
        byte[] data = Encoding.ASCII.GetBytes(message.ToTcp());

        await _stream!.WriteAsync(data, 0, data.Length, cancellationToken);
    }

    private async Task StartSender(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string? message = await Task.Run(Console.ReadLine, cancellationToken);
            
                if (message == null)
                {
                    continue;
                }

                string[] args = message.Split(" ");
            
                _ = Task.Run(async () =>
                {
                    try
                    {
                        List<IMessage> messagesToSend = await InterceptInput(args, cancellationToken);
                
                        foreach (IMessage messageToSend in messagesToSend)
                        {
                            await SendMessage(messageToSend, cancellationToken);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e);
                    }
                }, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task StartReceiver(NetworkStream stream, CancellationToken cancellationToken)
    {
        byte[] data = new byte[1024];

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int count = await stream.ReadAsync(data, 0, data.Length, cancellationToken);

                if (count == 0)
                {
                    break;
                }
                
                string response = Encoding.ASCII.GetString(data, 0, count);

                List<IMessage> messages = IMessage.ParseTcp(response);

                _ = Task.Run(async () =>
                {
                    List<IMessage> messagesToSend = await HandleResponse(messages, cancellationToken);
                    
                    foreach (IMessage messageToSend in messagesToSend)
                    {
                        await SendMessage(messageToSend, cancellationToken);
                    }
                }, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    public override void Stop()
    {
        _cancellationTokenSource.Cancel();
        
        _stream?.Close();
        _stream?.Dispose();
    }
}