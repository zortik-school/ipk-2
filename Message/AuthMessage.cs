namespace IPK_2.Message;

public record AuthMessage(string Username, string DisplayName, string Token) : IMessage
{
    public string ToTcp()
    {
        return $"AUTH {Username} AS {DisplayName} USING {Token}\r\n";
    }

    public byte[] ToUdp(byte[] messageId)
    {
        throw new NotImplementedException();
    }
}