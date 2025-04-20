using IPK_2.Chat;
using IPK_2.Message;

namespace IPK_2.Interceptor;

public class JoinInterceptor(ChatService service) : CommandInterceptor("join", 2, 2)
{
    public override Task<List<IMessage>> InterceptRequest(RequestContext context, CancellationToken cancellationToken)
    {
        List<IMessage> toSend = new List<IMessage>([new JoinMessage(context.Cmd[1], service.DisplayName)]);

        return Task.FromResult(toSend);
    }

    public new bool IsApplicable(string[] args)
    {
        return service.IsAuthenticated && base.IsApplicable(args);
    }

    public override Task<List<IMessage>> InterceptResponse(RequestContext? lastRequestContext, ResponseContext context,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<IMessage>());
    }
}