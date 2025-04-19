namespace IPK_2.Message;

public interface IMessage
{

    public static List<IMessage> ParseTcp(string message)
    {
        var result = new List<IMessage>();

        var lines = message.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            Func<string, IMessage?>[] parseFunctions = [ReplyMessage.Parse, MsgMessage.Parse, ErrorMessage.Parse];

            foreach (Func<string, IMessage?> parseFunc in parseFunctions)
            {
                IMessage? parsed = parseFunc(line);

                if (parsed == null)
                {
                    continue;
                }

                result.Add(parsed);
                    
                break;
            }
        }

        return result;
    }

    public static List<IMessage> ParseUdp(byte[] data)
    {
        
        // TODO
        return new List<IMessage>();
    }

    string ToTcp();

    byte[] ToUdp(byte[] messageId);
}