using IPK_2.Util;

namespace IPK_2.Message;

public record AuthMessage(string Username, string DisplayName, string Token, ushort? MessageId = null, ushort? RefMessageId = null) : IMessage
{
    public string ToTcp()
    {
        return $"AUTH {Username} AS {DisplayName} USING {Token}\r\n";
    }

    public byte[] ToUdp(byte[] messageId)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        bw.Write((byte) 0x02);
        bw.Write(messageId);
        BytesUtil.WriteStringBytes(bw, Username);
        BytesUtil.WriteStringBytes(bw, DisplayName);
        BytesUtil.WriteStringBytes(bw, Token);

        return ms.ToArray();
    }
}