using System.Text;

namespace IPK_2.Message;

public record ByeMessage(string From, ushort? MessageId = null, ushort? RefMessageId = null) : IMessage
{
    
    public static ByeMessage? ParseTcp(string message)
    {
        message = message.Replace("\r\n", "");
        
        string[] data = message.Split(" ");
        
        if (data.Length < 3 || !data[0].Equals("BYE") || !data[1].Equals("FROM"))
        {
            return null;
        }
        
        return new ByeMessage(data[2]);
    }
    
    public static ByeMessage? ParseUdp(byte[] message)
    {
        if (message[0] != 0xFF)
        {
            return null;
        }
        
        int index = 1;
        ushort? messageId = null;
        if (message.Length > index + 1)
        {
            messageId = BitConverter.ToUInt16(message, index);
            index += 2;
        }
        int displayNameEndIndex = Array.IndexOf(message, (byte)0, index);
        if (displayNameEndIndex == -1)
        {
            return null;
        }
        string displayName = Encoding.ASCII.GetString(message, index, displayNameEndIndex - index);
        return new ByeMessage(displayName, messageId);
    }
    
    public string ToTcp()
    {
        return $"BYE FROM {From}\r\n";
    }

    public byte[] ToUdp(byte[] messageId)
    {
        List<byte> byteArray = new List<byte>();
        byteArray.Add(0xFF);
        
        if (messageId != null && messageId.Length == 2)
        {
            byteArray.AddRange(messageId);
        }
        
        byte[] displayNameBytes = Encoding.ASCII.GetBytes(From);
        byteArray.AddRange(displayNameBytes);
        byteArray.Add(0x00);
        return byteArray.ToArray();
    }
}