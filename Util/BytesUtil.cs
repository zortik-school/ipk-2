using System.Text;

namespace IPK_2.Util;

public static class BytesUtil
{
    
    public static void WriteNullTerminatedString(BinaryWriter writer, string value)
    {
        byte[] strBytes = Encoding.ASCII.GetBytes(value);
        writer.Write(strBytes);
        writer.Write((byte) 0x00);
    }
    
    public static string ReadNullTerminatedString(BinaryReader reader)
    {
        List<byte> bytes = new();
        byte b;
        while ((b = reader.ReadByte()) != 0)
        {
            bytes.Add(b);
        }
        return Encoding.ASCII.GetString(bytes.ToArray());
    }
    
    public static void WriteStringBytes(BinaryWriter writer, string value)
    {
        writer.Write(Encoding.ASCII.GetBytes(value));
        writer.Write((byte) 0x00);
    }
    
    public static string ReadStringBytes(BinaryReader reader)
    {
        byte[] data = [];
            
        byte b;
        while ((b = reader.ReadByte()) != 0)
        {
            Array.Resize(ref data, data.Length + 1);
            
            data[^1] = b;
        }
        return Encoding.ASCII.GetString(data);
    }
}