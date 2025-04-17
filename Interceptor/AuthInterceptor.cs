using IPK_2.Chat;
using IPK_2.Message;

namespace IPK_2.Interceptor;

public class AuthInterceptor(ChatService service) : IFlowInterceptor
{
    public bool InterceptRequest(RequestContext context, Action<string> sendMessage)
    {
        string[] args = context.Cmd;

        sendMessage($"AUTH {args[1]} AS {args[3]} USING {args[2]}\r\n");

        return true;
    }

    public void InterceptResponse(RequestContext context, string response)
    {
        ReplyMessage reply = ReplyMessage.Parse(response);

        bool ok = reply.Process();

        service.IsAuthenticated = ok;

        if (ok)
        {
            service.DisplayName = context.Cmd[3];
        }
    }

    public bool IsApplicable(string[] args)
    {
        if (service.IsAuthenticated)
        {
            return false;
        }

        return args.Length > 3 && args[0].Equals("/auth");
    }
}