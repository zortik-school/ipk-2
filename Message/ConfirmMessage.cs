using System.Net;

namespace IPK_2.Message;

public record ConfirmMessage(ushort? RefMessageId, ushort? MessageId = null) : IMessage
{

    public static ConfirmMessage? ParseUdp(byte[] data)
    {
        using (var ms = new MemoryStream(data))
        using (var br = new BinaryReader(ms))
        {
            byte messageType = br.ReadByte();

            if (messageType != 0x00)
            {
                return null;
            }
            
            ushort refMessageId = (ushort) br.ReadInt16();

            return new ConfirmMessage(refMessageId);
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

        bw.Write((byte) 0x00);
        //bw.Write(messageId);
        bw.Write((short) RefMessageId!);

        return ms.ToArray();
    }

    public bool ExpectsConfirmation()
    {
        return false;
    }
}