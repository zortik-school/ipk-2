namespace IPK_2.Message;

public interface IMessage
{
    private static readonly Func<string, IMessage?>[] ParseFunctionsTcp =
    [
        ReplyMessage.ParseTcp,
        MsgMessage.ParseTcp,
        ErrorMessage.ParseTcp,
        ByeMessage.ParseTcp
    ];

    private static readonly Func<byte[], IMessage?>[] ParseFunctionsUdp =
    [
        ReplyMessage.ParseUdp,
        ConfirmMessage.ParseUdp,
        MsgMessage.ParseUdp,
        PingMessage.ParseUdp,
        ErrorMessage.ParseUdp
    ];

    ushort? MessageId { get; }

    ushort? RefMessageId { get; }

    public static List<IMessage> ParseTcp(string message)
    {
        var result = new List<IMessage>();

        var lines = message.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            foreach (Func<string, IMessage?> parseFunc in ParseFunctionsTcp)
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
        List<IMessage> result = new();
        
        foreach (var parseFunc in ParseFunctionsUdp)
        {
            IMessage? parsedMessage = parseFunc(data);

            if (parsedMessage != null)
            {
                result.Add(parsedMessage);
                break;
            }
        }

        return result;
    }

    string ToTcp();

    byte[] ToUdp(byte[] messageId);

    bool ExpectsConfirmation()
    {
        return true;
    }
}