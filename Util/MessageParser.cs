using IPK_2.Message;

namespace IPK_2.Util;

public class MessageParser
{
    public static ReplyMessage ParseReplyMessage(string message)
    {
        message = message.Replace("\r\n", "");
        
        string[] data = message.Split(" ");

        string content = string.Join(" ", data.Skip(3));

        return new ReplyMessage(data[1].Equals("OK"), content);
    }
}