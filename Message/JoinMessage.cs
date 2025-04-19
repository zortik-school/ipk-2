namespace IPK_2.Message;

public record JoinMessage(string ChannelName, string DisplayName) : IMessage
{
    public string ToTcp()
    {
        return $"JOIN {ChannelName} AS {DisplayName}\r\n";
    }

    public byte[] ToUdp(byte[] messageId)
    {
        throw new NotImplementedException();
    }
}