using System.Text;

namespace IPK_2.Util;

public static class BytesUtil
{
    
    public static void WriteNullTerminatedString(BinaryWriter writer, string value)
    {
        byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(value);
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
        return System.Text.Encoding.UTF8.GetString(bytes.ToArray());
    }
    
    public static void WriteStringBytes(BinaryWriter writer, string value)
    {
        writer.Write(Encoding.UTF8.GetBytes(value));
        writer.Write((byte) 0x00);
    }
    
    public static string ReadStringBytes(BinaryReader reader)
    {
        int length = reader.ReadByte();
        byte[] stringBytes = reader.ReadBytes(length);
        return Encoding.UTF8.GetString(stringBytes);
    }
}