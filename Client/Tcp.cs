using System.Net.Sockets;
using System.Text;
using IPK_2.Message;

namespace IPK_2.Client;

public class Tcp(string ip, int port) : SocketClient
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    
    public override void Start()
    {
        TcpClient client = new TcpClient(ip, port);
        NetworkStream stream = client.GetStream();
        
        Console.WriteLine("Connected to server.");
        
        CancellationToken cancellationToken = _cancellationTokenSource.Token;

        Parallel.Invoke(
            () => StartSender(stream, cancellationToken),
            () => StartReceiver(stream, cancellationToken)
        );
    }

    private void StartSender(NetworkStream stream, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            string? message = Console.ReadLine();
            
            if (message == null)
            {
                continue;
            }

            string[] args = message.Split(" ");
            
            Task.Run(() => InterceptInput(stream, args, cancellationToken), cancellationToken);
        }
    }

    private void StartReceiver(NetworkStream stream, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            byte[] data = new byte[1024];
            int count = stream.Read(data, 0, data.Length);
                
            string response = Encoding.UTF8.GetString(data, 0, count);
                
            Console.WriteLine($"Received from server: {response}");

            List<IMessage> messages = IMessage.Parse(response);

            Task.Run(() => HandleResponse(stream, messages, cancellationToken), cancellationToken);
        }
    }

    public override void Stop()
    {
        Console.WriteLine("Stopping...");
        
        _cancellationTokenSource.Cancel();
    }
}