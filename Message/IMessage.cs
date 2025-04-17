using IPK_2.Interceptor;

namespace IPK_2.Message;

public interface IMessage
{

    public static List<IMessage> Parse(string message)
    {
        var result = new List<IMessage>();

        var lines = message.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            IMessage? parsed;

            parsed = ReplyMessage.Parse(line);
            
            if (parsed == null)
            {
                parsed = MsgMessage.Parse(line);
            }

            if (parsed == null)
            {
                parsed = ErrorMessage.Parse(line);
            }

            if (parsed != null)
            {
                result.Add(parsed);
            }
        }

        return result;
    }
    
    void ProcessDefault(RequestContext context);
}