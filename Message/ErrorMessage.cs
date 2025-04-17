using IPK_2.Interceptor;

namespace IPK_2.Message;

public class ErrorMessage(string from, string messageContent) : IMessage
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
    
    public void ProcessDefault(RequestContext context)
    {
        Console.WriteLine($"Error from {from}: {messageContent}");
        
        context.Client.Stop();
    }
}