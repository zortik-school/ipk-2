using IPK_2.Client;

namespace IPK_2.Interceptor;

public record RequestContext(SocketClient Client, string[] Cmd) : IFlowContext;