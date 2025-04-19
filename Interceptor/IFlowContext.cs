using IPK_2.Client;

namespace IPK_2.Interceptor;

public interface IFlowContext
{
    SocketClient Client { get; }

}