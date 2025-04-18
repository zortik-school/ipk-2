namespace IPK_2.Interceptor;

public class HelpInterceptor() : CommandInterceptor("help", 1, 1)
{
    public Task InterceptRequest(RequestContext context, Action<string> sendMessage,
        CancellationToken cancellationToken)
    {
        foreach (IFlowInterceptor interceptor in context.Client.GetRequestInterceptors())
        {
            if (interceptor is CommandInterceptor commandInterceptor)
            {
                Console.WriteLine($"/{commandInterceptor.Name}");
            }
        }

        return Task.CompletedTask;
    }
}