using IPK_2.Message;

namespace IPK_2.Interceptor;

public interface IFlowInterceptor
{

    /**
     * Intercepts the command line arguments and sends something to the server.
     *
     * @param context The context of the request, including the command line arguments and the network stream.
     */
    Task<List<IMessage>> InterceptRequest(RequestContext context, CancellationToken cancellationToken)
    {
        List<IMessage> empty = new();
        
        return Task.FromResult(empty);
    }

    /**
     * Intercepts the response from the server.
     *
     * This is called when the InterceptRequest method returns true.
     *
     * @param context The context of the response.
     * @param response The response from server
     */
    Task InterceptResponse(RequestContext? lastRequestContext, ResponseContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /**
     * Checks if the interceptor is applicable for the given command line arguments.
     *
     * @param args The command line arguments.
     *
     * @return true if the interceptor is applicable, false otherwise
     */
    bool IsApplicable(string[] args)
    {
        return false;
    }
}