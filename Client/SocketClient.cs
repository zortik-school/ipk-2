using System.Text;
using IPK_2.Handler;
using IPK_2.Interceptor;

namespace IPK_2.Client;

public abstract class SocketClient
{
    private readonly List<IFlowInterceptor> _lineInterceptors = [];

    public abstract void Start();

    public abstract void Stop();
    
    public void RegisterRequestInterceptor(IFlowInterceptor interceptor)
    {
        _lineInterceptors.Add(interceptor);
    }

    protected IFlowInterceptor? InterceptInput(RequestContext context)
    {
        Action<string> sendMessageFunction = message =>
        {
            byte[] data = Encoding.ASCII.GetBytes(message);

            context.Stream.Write(data, 0, data.Length);
            
            Console.WriteLine("Sent to server: " + message);
        };
        
        foreach (IFlowInterceptor interceptor in _lineInterceptors)
        {
            if (interceptor.IsApplicable(context.Cmd) && interceptor.InterceptRequest(context, sendMessageFunction))
            {
                return interceptor;
            }
        }
        
        Console.WriteLine("No handler for this input");

        return null;
    }
    
    protected void HandleResponse(IFlowInterceptor interceptor, RequestContext context, string response)
    {
        interceptor.InterceptResponse(context, response);
    }
}