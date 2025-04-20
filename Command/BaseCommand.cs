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
            (t, s, p, d, r, h) =>
            {
                if (h)
                {
                    Console.Error.WriteLine("Help:");
                    Console.Error.WriteLine("  -t\tTransport protocol (tcp or udp)");
                    Console.Error.WriteLine("  -s\tServer IP address or hostname");
                    Console.Error.WriteLine("  -p\tServer port (default: 4567)");
                    Console.Error.WriteLine("  -d\tUDP confirmation timeout in ms (default: 250)");
                    Console.Error.WriteLine("  -r\tMaximum UDP retransmissions (default: 3)");
                    Console.Error.WriteLine("  -h\tShow this help");
                    return;
                }

                Console.Error.WriteLine($"Transport: {t}");
                Console.Error.WriteLine($"IP: {s}");
                Console.Error.WriteLine($"Port: {p}");
                Console.Error.WriteLine($"Timeout: {d}");
                Console.Error.WriteLine($"Retries: {r}");

                ChatService chatService = new ChatService();
                SocketClientProvider provider = new SocketClientProvider(chatService);

                SocketClient client;
                switch (t)
                {
                    case "tcp":
                        client = provider.GetTcpClient(s, p);
                        break;
                    case "udp":
                        client = provider.GetUdpClient(s, p);

                        (client as Udp)!.Retransmissions = r;
                        (client as Udp)!.Timeout = d;
                        break;
                    default:
                        Console.Error.WriteLine("Invalid transport protocol");
                        return;
                }

                client.Start();
            }
        );
    }
}