using IPK_2.Chat;
using IPK_2.Message;

namespace IPK_2.Interceptor;

public class AuthInterceptor(ChatService service) : CommandInterceptor("auth", 4, Int32.MaxValue)
{
    public override Task<List<IMessage>> InterceptRequest(RequestContext context, CancellationToken cancellationToken)
    {
        string[] args = context.Cmd;
        
        service.AwaitingAuth = true;

        List<IMessage> toSend = new([new AuthMessage(args[1], string.Join(" ", args.Skip(3)), args[2])]);
        
        return Task.FromResult(toSend);
    }

    public override Task<List<IMessage>> InterceptResponse(RequestContext? lastRequestContext, ResponseContext context, CancellationToken cancellationToken)
    {
        if (context.Message is not ReplyMessage reply || lastRequestContext == null || !service.AwaitingAuth)
        {
            return Task.FromResult(new List<IMessage>());
        }
        
        service.AwaitingAuth = false;
        service.IsAuthenticated = reply.Ok;
            
        if (reply.Ok)
        {
            service.DisplayName = lastRequestContext.Cmd[3];
        }

        return Task.FromResult(new List<IMessage>());
    }

    public new bool IsApplicable(string[] args)
    {
        return !service.IsAuthenticated && base.IsApplicable(args);
    }
}