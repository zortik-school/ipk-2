using IPK_2.Chat;
using IPK_2.Message;

namespace IPK_2.Interceptor;

public class MessageInterceptor(ChatService service) : IFlowInterceptor
{
    public Task<List<IMessage>> InterceptRequest(RequestContext context, CancellationToken cancellationToken)
    {
        List<IMessage> toSend =
            new List<IMessage>([new MsgMessage(service.DisplayName ?? "", string.Join(" ", context.Cmd))]);

        return Task.FromResult(toSend);
    }

    public Task<List<IMessage>> InterceptResponse(RequestContext? lastRequestContext, ResponseContext context, CancellationToken cancellationToken)
    {
        if (context.Message is MsgMessage msg)
        {
            Console.Write($"{msg.From}: {msg.MessageContent}\n");
        }
        
        return Task.FromResult(new List<IMessage>());
    }

    public bool IsApplicable(string[] args)
    {
        return service.IsAuthenticated && args.Length > 0 && !args[0].StartsWith("/");
    }
}