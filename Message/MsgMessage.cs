using IPK_2.Interceptor;
using IPK_2.Util;

namespace IPK_2.Message;

public record MsgMessage(string From, string MessageContent, ushort? MessageId = null, ushort? RefMessageId = null) : IMessage
{

    public static MsgMessage? ParseTcp(string message)
    {
        string[] data = message.Split(" ");

        if (data.Length < 5 || !data[0].Equals("MSG"))
        {
            return null;
        }

        return new MsgMessage(data[2], string.Join(" ", data.Skip(4)));
    }

    public static MsgMessage? ParseUdp(byte[] data)
    {
        using MemoryStream stream = new(data);
        using BinaryReader reader = new(stream);

        byte messageType = reader.ReadByte();
        if (messageType != 0x04)
        {
            return null;
        }

        ushort messageId = reader.ReadUInt16();

        string from = BytesUtil.ReadNullTerminatedString(reader);
        string messageContent = BytesUtil.ReadNullTerminatedString(reader);

        return new MsgMessage(from, messageContent, messageId);
    }

    public string ToTcp()
    {
        return $"MSG FROM {From} IS {MessageContent}\r\n";
    }

    public byte[] ToUdp(byte[] messageId)
    {
        if (messageId.Length != 2)
        {
            throw new ArgumentException("MessageId must be exactly 2 bytes long.");
        }

        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);

        writer.Write((byte) 0x04);
        writer.Write(messageId);

        BytesUtil.WriteNullTerminatedString(writer, From);
        BytesUtil.WriteNullTerminatedString(writer, MessageContent);

        return stream.ToArray();
    }
}