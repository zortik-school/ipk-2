using IPK_2.Util;

namespace IPK_2.Message;

public record ReplyMessage(bool Ok, string MessageContent, ushort? RefMessageId, ushort? MessageId = null) : IMessage
{

    public static ReplyMessage? ParseTcp(string message)
    {
        message = message.Replace("\r\n", "");
        
        string[] data = message.Split(" ");

        if (data.Length < 4 || !data[0].Equals("REPLY") || !data[2].Equals("IS"))
        {
            return null;
        }

        string content = string.Join(" ", data.Skip(3));

        return new ReplyMessage(data[1].Equals("OK"), content, null);
    }

    public static ReplyMessage? ParseUdp(byte[] data)
    {
        using (var ms = new MemoryStream(data))
        using (var br = new BinaryReader(ms))
        {
            byte messageType = br.ReadByte();

            if (messageType != 0x01)
            {
                return null;
            }
            
            ushort messageId = BitConverter.ToUInt16(br.ReadBytes(2), 0);

            byte okFlag = br.ReadByte();
            bool ok = okFlag == 0x01;

            ushort? refMessageId = null;
            if (ms.Position < ms.Length) 
            {
                refMessageId = (ushort?) br.ReadInt16();
            }

            string content = BytesUtil.ReadStringBytes(br);

            return new ReplyMessage(ok, content, refMessageId, messageId);
        }
    }

    public string ToTcp()
    {
        throw new NotImplementedException();
    }

    public byte[] ToUdp(byte[] messageId)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        bw.Write((byte) 0x01);
        bw.Write(messageId);
        bw.Write(Ok ? (byte) 0x01 : (byte) 0x00);
        bw.Write((short) RefMessageId!);
        BytesUtil.WriteStringBytes(bw, MessageContent);

        return ms.ToArray();
    }
}