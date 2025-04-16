using System.Text;
using IPK_2.Handler;
using IPK_2.Interceptor;

namespace IPK_2.Client;

public abstract class SocketClient
{
    private readonly List<IFlowInterceptor> _lineInterceptors = [];
    
    private readonly List<IIncomingMessageHandler> _handlers = [];

    public abstract void Start();
    
    public void RegisterRequestInterceptor(IFlowInterceptor interceptor)
    {
        _lineInterceptors.Add(interceptor);
    }

    public void RegisterIncomingMessageHandler(IIncomingMessageHandler handler)
    {
        _handlers.Add(handler);
    }

    protected bool InterceptInput(RequestContext context)
    {
        Action<string> sendMessageFunction = message =>
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            context.Stream.Write(data, 0, data.Length);
        };
        
        foreach (IFlowInterceptor interceptor in _lineInterceptors)
        {
            if (interceptor.IsApplicable(context.Cmd))
            {
                return interceptor.InterceptRequest(context, sendMessageFunction);
            }
        }

        return false;
    }
    
    protected void HandleResponse(RequestContext context, string response)
    {
        foreach (IFlowInterceptor interceptor in _lineInterceptors)
        {
            if (interceptor.IsApplicable(context.Cmd))
            {
                interceptor.InterceptResponse(context, response);
            }
        }
    }

    protected void HandleIncomingMessage(string message)
    {
        foreach (IIncomingMessageHandler handler in _handlers)
        {
            if (handler.IsApplicable(message))
            {
                handler.HandleIncomingMessage(message);

                return;
            }
        }
    }
}