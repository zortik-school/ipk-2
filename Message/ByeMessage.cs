namespace IPK_2.Message;

public record ByeMessage(string From, ushort? MessageId = null, ushort? RefMessageId = null) : IMessage
{
    
    public static ByeMessage? ParseTcp(string message)
    {
        message = message.Replace("\r\n", "");
        
        string[] data = message.Split(" ");
        
        if (data.Length < 3 || !data[0].Equals("BYE") || !data[1].Equals("FROM"))
        {
            return null;
        }
        
        return new ByeMessage(data[2]);
    }
    
    public string ToTcp()
    {
        return $"BYE FROM {From}\r\n";
    }

    public byte[] ToUdp(byte[] messageId)
    {
        throw new NotImplementedException();
    }
}