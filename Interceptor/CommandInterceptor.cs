namespace IPK_2.Interceptor;

public abstract class CommandInterceptor(string name, int minArgs, int maxArgs) : IFlowInterceptor
{
    public string Name => name;
    
    public bool IsApplicable(string[] args)
    {
        return args.Length >= minArgs && args.Length <= maxArgs && args[0].Equals($"/{name}");
    }
}