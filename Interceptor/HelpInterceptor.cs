using IPK_2.Message;

namespace IPK_2.Interceptor;

public class HelpInterceptor() : CommandInterceptor("help", 1, 1)
{
    public override Task<List<IMessage>> InterceptRequest(RequestContext context, CancellationToken cancellationToken)
    {
        foreach (IFlowInterceptor interceptor in context.Client.GetRequestInterceptors())
        {
            if (interceptor is CommandInterceptor commandInterceptor)
            {
                Console.WriteLine($"/{commandInterceptor.Name}");
            }
        }

        return Task.FromResult(new List<IMessage>());
    }

    public override Task<List<IMessage>> InterceptResponse(RequestContext? lastRequestContext, ResponseContext context,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<IMessage>());
    }
}