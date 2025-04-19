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

    protected async Task<List<IMessage>> InterceptInput(string[] args, CancellationToken cancellationToken)
    {
        RequestContext context = new RequestContext(this, args);
        
        bool handled = false;

        List<IMessage> messages = [];
        
        foreach (IFlowInterceptor interceptor in _lineInterceptors)
        {
            if (interceptor.IsApplicable(context.Cmd))
            {
                List<IMessage> messagesToSend = await interceptor.InterceptRequest(context, cancellationToken);
                
                messages.AddRange(messagesToSend);

                _lastRequestContexts[interceptor] = context;

                handled = true;
            }
        }

        if (!handled)
        {
            Console.WriteLine("No handler for this input");
        }

        return messages;
    }
    
    protected async Task HandleResponse(List<IMessage> messages, CancellationToken cancellationToken)
    {
        try
        {
            foreach (IMessage message in messages)
            {
                ResponseContext context = new ResponseContext(this, message);
            
                foreach (IFlowInterceptor interceptor in _lineInterceptors)
                {
                    RequestContext? requestContext = GetLastRequestContext(interceptor);
                
                    await interceptor.InterceptResponse(requestContext, context, cancellationToken);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}