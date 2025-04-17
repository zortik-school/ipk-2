using IPK_2.Chat;
using IPK_2.Message;

namespace IPK_2.Interceptor;

public class JoinInterceptor(ChatService service) : IFlowInterceptor
{
    public bool InterceptRequest(RequestContext context, Action<string> sendMessage)
    {
        sendMessage($"JOIN {context.Cmd[1]} {service.DisplayName}\r\n");

        return true;
    }

    public void InterceptResponse(RequestContext context, string response)
    {
        ReplyMessage.Parse(response).Process();
    }

    public bool IsApplicable(string[] args)
    {
        return service.IsAuthenticated && args is ["/join", _];
    }
}