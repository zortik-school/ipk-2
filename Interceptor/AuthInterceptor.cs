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

    public bool InterceptResponse(RequestContext context, IMessage response)
    {
        if (response is ReplyMessage reply)
        {
            reply.ProcessDefault(context);
            
            service.IsAuthenticated = reply.Ok;
            
            if (reply.Ok)
            {
                service.DisplayName = context.Cmd[3];
            }

            return true;
        }

        return false;
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