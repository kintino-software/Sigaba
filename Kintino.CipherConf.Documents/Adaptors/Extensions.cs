using System.Text;

namespace Kintino.CipherConf.Documents.Adaptors;

internal static class Extensions
{
    extension(string str)
    {
        public byte[] ToUTF8Bytes()
        {
            return Encoding.UTF8.GetBytes(str);
        }
    }

    extension(byte[] data)
    {
        public string ToUTF8String()
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
