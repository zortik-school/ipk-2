namespace IPK_2.Message;

public record PingMessage(ushort? RefMessageId = null, ushort? MessageId = null) : IMessage
{

    public static PingMessage? ParseUdp(byte[] data)
    {
        if (data.Length != 3 || data[0] != 0xFD)
        {
            return null;
        }

        using MemoryStream stream = new(data);
        using BinaryReader reader = new(stream);

        reader.ReadByte();
        ushort messageId = reader.ReadUInt16();

        return new PingMessage(null, messageId);
    }
    
    public string ToTcp()
    {
        throw new NotImplementedException();
    }

    public byte[] ToUdp(byte[] messageId)
    {
        if (messageId.Length != 2)
        {
            throw new ArgumentException("MessageId must be exactly 2 bytes.");
        }

        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);

        writer.Write((byte)0xFD);
        writer.Write(messageId);

        return stream.ToArray();
    }

    public bool ExpectsConfirmation()
    {
        return true;
    }
}