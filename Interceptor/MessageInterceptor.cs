using IPK_2.Chat;

namespace IPK_2.Interceptor;

public class MessageInterceptor(ChatService service) : IFlowInterceptor
{
    public bool InterceptRequest(RequestContext context, Action<string> sendMessage)
    {
        sendMessage($"MSG FROM {service.DisplayName} IS {string.Join(" ", context.Cmd)}\r\n");

        return true;
    }

    public bool IsApplicable(string[] args)
    {
        return service.IsAuthenticated && args.Length > 0;
    }
}