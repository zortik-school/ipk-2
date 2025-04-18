using IPK_2.Chat;
using IPK_2.Message;

namespace IPK_2.Interceptor;

public class MessageInterceptor(ChatService service) : IFlowInterceptor
{
    public Task InterceptRequest(RequestContext context, Action<string> sendMessage, CancellationToken cancellationToken)
    {
        sendMessage($"MSG FROM {service.DisplayName} IS {string.Join(" ", context.Cmd)}\r\n");

        return Task.CompletedTask;
    }

    public Task InterceptResponse(RequestContext? lastRequestContext, ResponseContext context, CancellationToken cancellationToken)
    {
        if (context.Message is MsgMessage msg)
        {
            Console.WriteLine($"Message from {msg.From}: {msg.MessageContent}");
        }
        
        return Task.CompletedTask;
    }

    public bool IsApplicable(string[] args)
    {
        return service.IsAuthenticated && args.Length > 0 && !args[0].StartsWith("/");
    }
}