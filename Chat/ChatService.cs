namespace IPK_2.Chat;

public class ChatService
{
    public bool AwaitingAuth { get; set; } = false;
    public bool IsAuthenticated { get; set; } = false;
    public string? DisplayName { get; set; } = null;

    public void Close()
    {
        // TODO
    }
}