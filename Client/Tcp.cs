using System.Net.Sockets;
using System.Text;
using IPK_2.Interceptor;

namespace IPK_2.Client;

public class Tcp(string ip, int port) : SocketClient
{
    public override void Start()
    {
        StartSender();
    }

    private void StartSender()
    {
        Parallel.Invoke(() =>
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

                Dictionary<string, object> metaOfThisFlow = new();

                RequestContext context = new RequestContext(this, args, stream, metaOfThisFlow);

                IFlowInterceptor? appliedInterceptor = InterceptInput(context);
            
                if (appliedInterceptor != null)
                {
                    byte[] data = new byte[1024];
                    int count = stream.Read(data, 0, data.Length);
                
                    string response = Encoding.UTF8.GetString(data, 0, count);
                
                    Console.WriteLine($"Received from server: {response}");
                
                    HandleResponse(appliedInterceptor, context, response);
                }
            }
        });
    }

    public override void Stop()
    {
        throw new NotImplementedException();
    }
}