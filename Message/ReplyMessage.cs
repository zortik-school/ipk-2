namespace IPK_2.Message;

public record ReplyMessage(bool Ok, string MessageContent) : IMessage
{

    public static ReplyMessage? Parse(string message)
    {
        message = message.Replace("\r\n", "");
        
        string[] data = message.Split(" ");

        if (data.Length < 4 || !data[0].Equals("REPLY") || !data[2].Equals("IS"))
        {
            return null;
        }

        string content = string.Join(" ", data.Skip(3));

        return new ReplyMessage(data[1].Equals("OK"), content);
    }

    public string ToTcp()
    {
        throw new NotImplementedException();
    }

    public byte[] ToUdp(byte[] messageId)
    {
        throw new NotImplementedException();
    }
}