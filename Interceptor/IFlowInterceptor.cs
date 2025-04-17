using IPK_2.Message;

namespace IPK_2.Interceptor;

public interface IFlowInterceptor
{
    
    /**
     * Intercepts the command line arguments and sends something to the server.
     *
     * @param context The context of the request, including the command line arguments and the network stream.
     * @param sendMessage The function to send a message to the server.
     *
     * @return true if the client should wait for a response from the server, false otherwise
     */
    bool InterceptRequest(RequestContext context, Action<string> sendMessage);

    /**
     * Intercepts the response from the server.
     *
     * This is called when the InterceptRequest method returns true.
     *
     * @param context The context of the original request. The same, ass used in InterceptRequest
     * @param response The response from server
     * @return true if the message was handled by this interceptor. otherwise, it should be handled
     * by the IMessage.Parse method.
     */
    bool InterceptResponse(RequestContext context, IMessage response)
    {
        response.ProcessDefault(context);

        return true;
    }
    
    /**
     * Checks if the interceptor is applicable for the given command line arguments.
     *
     * @param args The command line arguments.
     *
     * @return true if the interceptor is applicable, false otherwise
     */
    bool IsApplicable(string[] args);
}