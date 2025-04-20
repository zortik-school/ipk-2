using IPK_2.Message;

namespace IPK_2.Interceptor;

public abstract class CommandInterceptor(string name, int minArgs, int maxArgs) : IFlowInterceptor
{
    public string Name => name;
    
    public abstract Task<List<IMessage>> InterceptRequest(RequestContext context, CancellationToken cancellationToken);

    public abstract Task<List<IMessage>> InterceptResponse(RequestContext? lastRequestContext, ResponseContext context,
        CancellationToken cancellationToken);
    
    public bool IsApplicable(string[] args)
    {
        return args.Length >= minArgs && args.Length <= maxArgs && args[0].Equals($"/{name}");
    }
}