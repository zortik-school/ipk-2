using IPK_2.Interceptor;

namespace IPK_2.Message;

public record MsgMessage(string From, string MessageContent) : IMessage
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
    
    public void ProcessDefault(IFlowContext context)
    {
        Console.WriteLine($"Message from {From}: {MessageContent}");
    }
}