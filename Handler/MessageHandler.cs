using IPK_2.Chat;

namespace IPK_2.Handler;

public class MessageHandler(ChatService service) : IIncomingMessageHandler
{
    public void HandleIncomingMessage(string message)
    {
        throw new NotImplementedException();
    }

    public bool IsApplicable(string message)
    {
        return service.IsAuthenticated;
    }
}