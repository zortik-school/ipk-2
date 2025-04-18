using System.Net.Sockets;
using IPK_2.Client;
using IPK_2.Message;

namespace IPK_2.Interceptor;

public record ResponseContext(SocketClient Client, NetworkStream Stream, IMessage Message) : IFlowContext;