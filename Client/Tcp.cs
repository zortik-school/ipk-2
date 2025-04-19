using System.Net.Sockets;
using System.Text;
using IPK_2.Message;
using IPK_2.Util;

namespace IPK_2.Client;

public class Tcp(string ip, int port) : SocketClient
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private NetworkStream? _stream;
    
    public override void Start()
    {
        TcpClient client = new TcpClient(ip, port);
        _stream = client.GetStream();
        
        Console.WriteLine("Connected to server.");
        
        CancellationToken cancellationToken = _cancellationTokenSource.Token;

        try
        {
            Task.WhenAll(
                StartSender(cancellationToken),
                StartReceiver(_stream, cancellationToken)).Wait(cancellationToken);
        }
        catch (OperationCanceledException e)
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
            
                Task.Run(async () =>
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
                        Console.WriteLine(e);
                    }
                }, cancellationToken);
            }
        }
        catch (OperationCanceledException e)
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
                
                string response = Encoding.UTF8.GetString(data, 0, count);

                List<IMessage> messages = IMessage.ParseTcp(response);

                Task.Run(() => HandleResponse(messages, cancellationToken), cancellationToken);
            }
        }
        catch (OperationCanceledException e)
        {
        }
    }

    public override void Stop()
    {
        Console.WriteLine("Stopping...");
        
        _cancellationTokenSource.Cancel();
        
        _stream?.Close();
        _stream?.Dispose();
    }
}