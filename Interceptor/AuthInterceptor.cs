using IPK_2.Chat;
using IPK_2.Message;

namespace IPK_2.Interceptor;

public class AuthInterceptor(ChatService service) : CommandInterceptor("auth", 4, Int32.MaxValue)
{
    public Task InterceptRequest(RequestContext context, Action<string> sendMessage, CancellationToken cancellationToken)
    {
        string[] args = context.Cmd;
        
        service.AwaitingAuth = true;

        sendMessage($"AUTH {args[1]} AS {args[3]} USING {args[2]}\r\n");
        return Task.CompletedTask;
    }

    public Task InterceptResponse(RequestContext? lastRequestContext, ResponseContext context, CancellationToken cancellationToken)
    {
        if (context.Message is not ReplyMessage reply || lastRequestContext == null || !service.AwaitingAuth)
        {
            return Task.CompletedTask;
        }
        
        service.AwaitingAuth = false;
        service.IsAuthenticated = reply.Ok;
            
        if (reply.Ok)
        {
            service.DisplayName = lastRequestContext.Cmd[3];
        }

        return Task.CompletedTask;
    }

    public new bool IsApplicable(string[] args)
    {
        return !service.IsAuthenticated && base.IsApplicable(args);
    }
}