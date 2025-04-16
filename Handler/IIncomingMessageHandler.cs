namespace IPK_2.Handler;

public interface IIncomingMessageHandler
{
    
    void HandleIncomingMessage(string message);

    bool IsApplicable(string message);
}