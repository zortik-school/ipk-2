using IPK_2.Message;

namespace IPK_2.Interceptor;

public class ReplyInterceptor : IFlowInterceptor
{
    public Task InterceptResponse(RequestContext? lastRequestContext, ResponseContext context,
        CancellationToken cancellationToken)
    {
        if (context.Message is ReplyMessage reply)
        {
            string state = reply.Ok ? "Success" : "Failure";
        
            Console.Write($"Action {state}: {reply.MessageContent}\n");
        }
        
        return Task.CompletedTask;
    }
}