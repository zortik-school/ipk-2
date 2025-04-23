using System.Text;

namespace IPK_2.Message;

public record JoinMessage(string ChannelName, string DisplayName, ushort? MessageId = null, ushort? RefMessageId = null) : IMessage
{
    public string ToTcp()
    {
        return $"JOIN {ChannelName} AS {DisplayName}\r\n";
    }

    public byte[] ToUdp(byte[] messageId)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        bw.Write((byte) 0x03);
        bw.Write(messageId);

        // ChannelID
        bw.Write(Encoding.ASCII.GetBytes(ChannelName));
        bw.Write((byte) 0x00);

        // DisplayName
        bw.Write(Encoding.ASCII.GetBytes(DisplayName));
        bw.Write((byte) 0x00);

        return ms.ToArray();
    }
}