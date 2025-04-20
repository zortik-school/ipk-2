using IPK_2.Chat;
using IPK_2.Message;

namespace IPK_2.Interceptor;

public class RenameInterceptor(ChatService service) : CommandInterceptor("rename", 2, Int32.MaxValue)
{
    public override Task<List<IMessage>> InterceptRequest(RequestContext context, CancellationToken cancellationToken)
    {
        service.DisplayName = string.Join(" ", context.Cmd.Skip(1));
        
        return Task.FromResult(new List<IMessage>());
    }

    public override Task<List<IMessage>> InterceptResponse(RequestContext? lastRequestContext, ResponseContext context,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<IMessage>());
    }

    public new bool IsApplicable(string[] args)
    {
        return service.IsAuthenticated && base.IsApplicable(args);
    }
}