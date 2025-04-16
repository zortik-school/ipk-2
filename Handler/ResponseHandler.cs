namespace IPK_2.Handler;

public interface IResponseHandler
{
    
    /**
     * Handles the response from the server.
     *
     * @param cmd The command line arguments that were passed before this reply.
     * @param response The response from the server.
     */
    void OnResponseReceived(RequestContext request, string response);
}