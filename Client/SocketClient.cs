using System.Net.Sockets;
using System.Text;
using IPK_2.Interceptor;
using IPK_2.Message;

namespace IPK_2.Client;

public abstract class SocketClient
{
    private readonly List<IFlowInterceptor> _lineInterceptors = [];
    private readonly Dictionary<IFlowInterceptor, RequestContext> _lastRequestContexts = new();

    public abstract void Start();

    public abstract void Stop();
    
    public void RegisterRequestInterceptor(IFlowInterceptor interceptor)
    {
        _lineInterceptors.Add(interceptor);
    }

    public List<IFlowInterceptor> GetRequestInterceptors()
    {
        return [.._lineInterceptors];
    }

    private RequestContext? GetLastRequestContext(IFlowInterceptor interceptor)
    {
        return _lastRequestContexts.GetValueOrDefault(interceptor);
    }

    protected async Task InterceptInput(NetworkStream stream, string[] args, CancellationToken cancellationToken)
    {
        RequestContext context = new RequestContext(this, args, stream);
        
        Action<string> sendMessageFunction = message =>
        {
            byte[] data = Encoding.ASCII.GetBytes(message);

            context.Stream.WriteAsync(data, 0, data.Length, cancellationToken);
            
            Console.WriteLine("Sent to server: " + message);
        };
        
        bool handled = false;
        
        foreach (IFlowInterceptor interceptor in _lineInterceptors)
        {
            if (interceptor.IsApplicable(context.Cmd))
            {
                await interceptor.InterceptRequest(context, sendMessageFunction, cancellationToken);
                
                _lastRequestContexts.Add(interceptor, context);

                handled = true;
            }
        }

        if (!handled)
        {
            Console.WriteLine("No handler for this input");
        }
    }
    
    protected async Task HandleResponse(NetworkStream stream, List<IMessage> messages, CancellationToken cancellationToken)
    {
        foreach (IMessage message in messages)
        {
            ResponseContext context = new ResponseContext(this, stream, message);
            
            foreach (IFlowInterceptor interceptor in _lineInterceptors)
            {
                RequestContext? requestContext = GetLastRequestContext(interceptor);
                
                await interceptor.InterceptResponse(requestContext, context, cancellationToken);
            }
        }
    }
}