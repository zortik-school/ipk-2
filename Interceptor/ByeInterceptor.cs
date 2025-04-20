using IPK_2.Message;

namespace IPK_2.Interceptor;

public class ByeInterceptor : IFlowInterceptor
{
    public Task<List<IMessage>> InterceptResponse(RequestContext? lastRequestContext, ResponseContext context,
        CancellationToken cancellationToken)
    {
        if (context.Message is ByeMessage byeMessage)
        {
            context.Client.Stop();
        }

        return Task.FromResult(new List<IMessage>());
    }
}