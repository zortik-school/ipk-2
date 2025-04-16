using IPK_2.Chat;

namespace IPK_2.Interceptor;

public class AuthInterceptor(ChatService service) : IFlowInterceptor
{
    public bool InterceptRequest(RequestContext context, Action<string> sendMessageFunction)
    {
        throw new NotImplementedException();
    }

    public void InterceptResponse(RequestContext context, string response)
    {
        throw new NotImplementedException();
    }

    public bool IsApplicable(string[] args)
    {
        if (service.IsAuthenticated)
        {
            return false;
        }
        
        throw new NotImplementedException();
    }
}