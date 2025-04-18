using IPK_2.Message;

namespace IPK_2.Interceptor;

public class ErrorInterceptor : IFlowInterceptor
{
    public Task InterceptResponse(RequestContext? lastRequestContext, ResponseContext context, CancellationToken cancellationToken)
    {
        if (context.Message is ErrorMessage error)
        {
            Console.WriteLine($"Error from {error.From}: {error.MessageContent}");
        
            context.Client.Stop();
        }
        
        return Task.CompletedTask;
    }
}