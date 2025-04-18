namespace IPK_2.Message;

public interface IMessage
{

    public static List<IMessage> Parse(string message)
    {
        var result = new List<IMessage>();

        var lines = message.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            IMessage? parsed = null;

            if (parsed == null)
            {
                parsed = ReplyMessage.Parse(line);
            }
            
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
}