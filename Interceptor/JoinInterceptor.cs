using IPK_2.Chat;

namespace IPK_2.Interceptor;

public class JoinInterceptor(ChatService service) : IFlowInterceptor
{
    public bool InterceptRequest(RequestContext context, Action<string> sendMessage)
    {
        sendMessage($"JOIN {context.Cmd[1]} AS {service.DisplayName}\r\n");

        return true;
    }

    public bool IsApplicable(string[] args)
    {
        return service.IsAuthenticated && args is ["/join", _];
    }
}