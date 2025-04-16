using System.Net.Sockets;
using System.Text;
using IPK_2.Handler;
using IPK_2.Interceptor;

namespace IPK_2;

public abstract class SocketClient
{
    private readonly List<ICommandLineInterceptor> _lineInterceptors = new();
    private readonly List<IResponseHandler> _responseHandlers = new();

    public abstract void Start();
    
    public void RegisterRequestInterceptor(ICommandLineInterceptor interceptor)
    {
        _lineInterceptors.Add(interceptor);
    }
    
    public void RegisterResponseHandler(IResponseHandler handler)
    {
        _responseHandlers.Add(handler);
    }

    protected bool InterceptInput(string[] args, NetworkStream stream)
    {
        Action<string> sendMessageFunction = message =>
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            stream.Write(data, 0, data.Length);
        };
        
        foreach (ICommandLineInterceptor interceptor in _lineInterceptors)
        {
            if (interceptor.IsApplicable(args))
            {
                return interceptor.Intercept(args, sendMessageFunction, stream);
            }
        }

        return false;
    }
    
    protected void HandleResponse(string[] args, string response)
    {
        RequestContext context = new RequestContext(args);
        
        foreach (IResponseHandler handler in _responseHandlers)
        {
            handler.OnResponseReceived(context, response);
        }
    }
}