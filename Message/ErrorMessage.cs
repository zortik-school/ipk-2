using IPK_2.Util;

namespace IPK_2.Message;

public record ErrorMessage(string From, string MessageContent, ushort? MessageId = null, ushort? RefMessageId = null) : IMessage
{

    public static ErrorMessage? ParseTcp(string message)
    {
        string[] data = message.Split(" ");

        if (data.Length < 5 || !data[0].Equals("ERR"))
        {
            return null;
        }

        return new ErrorMessage(data[2], string.Join(" ", data.Skip(4)));
    }

    public static ErrorMessage? ParseUdp(byte[] data)
    {
        if (data.Length < 4 || data[0] != 0xFE)
        {
            return null;
        }

        using MemoryStream stream = new(data);
        using BinaryReader reader = new(stream);

        reader.ReadByte();
        ushort messageId = reader.ReadUInt16();

        string from = BytesUtil.ReadNullTerminatedString(reader);
        string messageContent = BytesUtil.ReadNullTerminatedString(reader);

        return new ErrorMessage(from, messageContent, messageId);
    }

    public string ToTcp()
    {
        return $"ERROR FROM {From}: {MessageContent}\n";
    }

    public byte[] ToUdp(byte[] messageId)
    {
        if (messageId.Length != 2)
        {
            throw new ArgumentException("MessageId must be exactly 2 bytes.");
        }

        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);

        writer.Write((byte) 0xFE);
        writer.Write(messageId);
        BytesUtil.WriteNullTerminatedString(writer, From);
        BytesUtil.WriteNullTerminatedString(writer, MessageContent);

        return stream.ToArray();
    }
}