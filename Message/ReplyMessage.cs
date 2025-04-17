namespace IPK_2.Message;

public record ReplyMessage(bool Ok, string MessageContent)
{

    public static ReplyMessage Parse(string message)
    {
        message = message.Replace("\r\n", "");
        
        string[] data = message.Split(" ");

        string content = string.Join(" ", data.Skip(3));

        return new ReplyMessage(data[1].Equals("OK"), content);
    }

    public bool Process()
    {
        Console.WriteLine($"Reply: {MessageContent}");
        
        return Ok;
    }
}