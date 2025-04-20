using IPK_2.Message;

namespace IPK_2.Interceptor;

public class ErrorInterceptor : IFlowInterceptor
{
    public Task<List<IMessage>> InterceptResponse(RequestContext? lastRequestContext, ResponseContext context, CancellationToken cancellationToken)
    {
        if (context.Message is ErrorMessage error)
        {
            Console.Write(error.ToTcp());
        
            context.Client.Stop();
        }
        
        return Task.FromResult(new List<IMessage>());
    }
}