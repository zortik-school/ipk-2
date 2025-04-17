using IPK_2.Interceptor;

namespace IPK_2.Message;

public class MsgMessage(string from, string messageContent) : IMessage
{

    public static MsgMessage? Parse(string message)
    {
        string[] data = message.Split(" ");

        if (data.Length < 5 || !data[0].Equals("MSG"))
        {
            return null;
        }

        return new MsgMessage(data[2], string.Join(" ", data.Skip(4)));
    }
    
    public void ProcessDefault(RequestContext context)
    {
        Console.WriteLine($"Message from {from}: {messageContent}");
    }
}