namespace IPK_2.Client;

public static class SocketClientProvider
{
    public static SocketClient GetTcpClient(string ip, int port)
    {
        SocketClient client = new Tcp(ip, port);
        
        InitClient(client);

        return client;
    }

    public static SocketClient GetUdpClient()
    {
        // TODO
        return null;
    }

    private static void InitClient(SocketClient client)
    {
        
        // TODO
    }
}