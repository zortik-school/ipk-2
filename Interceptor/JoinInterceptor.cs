using IPK_2.Chat;

namespace IPK_2.Interceptor;

public class JoinInterceptor(ChatService service) : IFlowInterceptor
{
    public Task InterceptRequest(RequestContext context, Action<string> sendMessage, CancellationToken cancellationToken)
    {
        sendMessage($"JOIN {context.Cmd[1]} AS {service.DisplayName}\r\n");
        
        return Task.CompletedTask;
    }

    public bool IsApplicable(string[] args)
    {
        return service.IsAuthenticated && args is ["/join", _];
    }
}