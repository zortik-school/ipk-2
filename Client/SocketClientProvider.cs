using IPK_2.Chat;
using IPK_2.Interceptor;

namespace IPK_2.Client;

public class SocketClientProvider(ChatService chatService)
{
    public SocketClient GetTcpClient(string ip, int port)
    {
        SocketClient client = new Tcp(ip, port);
        
        InitClient(client);

        return client;
    }

    public SocketClient GetUdpClient()
    {
        // TODO
        return null;
    }

    private void InitClient(SocketClient client)
    {
        client.RegisterRequestInterceptor(new AuthInterceptor(chatService));
        client.RegisterRequestInterceptor(new JoinInterceptor(chatService));
        client.RegisterRequestInterceptor(new MessageInterceptor(chatService));
        client.RegisterRequestInterceptor(new RenameInterceptor(chatService));
        client.RegisterRequestInterceptor(new HelpInterceptor());
        client.RegisterRequestInterceptor(new ErrorInterceptor());
    }
}