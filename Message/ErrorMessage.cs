namespace IPK_2.Message;

public record ErrorMessage(string From, string MessageContent) : IMessage
{

    public static ErrorMessage? Parse(string message)
    {
        string[] data = message.Split(" ");

        if (data.Length < 5 || !data[0].Equals("ERR"))
        {
            return null;
        }

        return new ErrorMessage(data[2], string.Join(" ", data.Skip(4)));
    }

    public string ToTcp()
    {
        return $"ERROR FROM {From}: {MessageContent}\n";
    }

    public byte[] ToUdp(byte[] messageId)
    {
        throw new NotImplementedException();
    }
}