using System.Net.Sockets;
using System.Text;

namespace IPK_2.Client;

public class Tcp(string ip, int port) : SocketClient
{
    public override void Start()
    {
        TcpClient client = new TcpClient(ip, port);
        
        NetworkStream stream = client.GetStream();
        
        Console.WriteLine("Connected to server.");

        while (true)
        {
            string? message = Console.ReadLine();
            
            if (message == null)
            {
                continue;
            }

            string[] args = message.Split(" ");

            bool shouldWait = InterceptInput(args, stream);
            
            if (shouldWait)
            {
                byte[] data = new byte[1024];
                int bytesRead = stream.Read(data, 0, data.Length);
                
                string response = Encoding.UTF8.GetString(data, 0, bytesRead);
                
                Console.WriteLine($"Received from server: {response}");
                
                HandleResponse(args, response);
            }
        }
    }
}