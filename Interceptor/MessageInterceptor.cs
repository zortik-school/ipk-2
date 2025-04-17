using IPK_2.Chat;
using IPK_2.Message;

namespace IPK_2.Interceptor;

public class MessageInterceptor(ChatService service) : IFlowInterceptor
{
    public bool InterceptRequest(RequestContext context, Action<string> sendMessage)
    {
        sendMessage($"MSG {service.DisplayName} {string.Join(" ", context.Cmd)}\r\n");

        return true;
    }

    public void InterceptResponse(RequestContext context, string response)
    {
        ReplyMessage.Parse(response).Process();
    }

    public bool IsApplicable(string[] args)
    {
        return service.IsAuthenticated && args.Length > 0;
    }
}