using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using IPK_2.Chat;
using IPK_2.Client;

namespace IPK_2.Command;

public class BaseCommand : ICommand
{
    public Option[] GetOptions() =>
    [
        new Option<string>(
            aliases: ["-t"],
            description: "Transport protocol used for connection (tcp or udp)")
        {
            IsRequired = true
        },
        new Option<string>(
            aliases: ["-s"],
            description: "Server IP address or hostname")
        {
            IsRequired = true
        },
        new Option<ushort>(
            aliases: ["-p"],
            () => 4567,
            description: "Server port"
        ),
        new Option<ushort>(
            aliases: ["-d"],
            () => 250,
            description: "UDP confirmation timeout (in milliseconds)"
        ),
        new Option<byte>(
            aliases: ["-r"],
            () => 3,
            description: "Maximum number of UDP retransmissions"
        ),
        new Option<bool>(
            aliases: ["-h"],
            description: "Prints program help output and exits"
        )
    ];

    public string GetDescription() => "Client for connecting to a chat server using TCP or UDP.";

    public ICommandHandler GetHandler()
    {
        return CommandHandler.Create<string, string, ushort, ushort, byte, bool>(
            (transport, ip, port, timeout, retries, help) =>
            {
                if (help)
                {
                    Console.WriteLine("Help:");
                    Console.WriteLine("  -t\tTransport protocol (tcp or udp)");
                    Console.WriteLine("  -s\tServer IP address or hostname");
                    Console.WriteLine("  -p\tServer port (default: 4567)");
                    Console.WriteLine("  -d\tUDP confirmation timeout in ms (default: 250)");
                    Console.WriteLine("  -r\tMaximum UDP retransmissions (default: 3)");
                    Console.WriteLine("  -h\tShow this help");
                    return;
                }

                Console.WriteLine($"Transport: {transport}");
                Console.WriteLine($"IP: {ip}");
                Console.WriteLine($"Port: {port}");
                Console.WriteLine($"Timeout: {timeout}");
                Console.WriteLine($"Retries: {retries}");

                ChatService chatService = new ChatService();
                SocketClientProvider provider = new SocketClientProvider(chatService);

                SocketClient client;
                switch (transport)
                {
                    case "tcp":
                        client = provider.GetTcpClient(ip, port);
                        break;
                    case "udp":
                        client = provider.GetUdpClient();
                        break;
                    default:
                        // TODO: Message for invalid transport
                        return;
                }

                client.Start();
            }
        );
    }
}