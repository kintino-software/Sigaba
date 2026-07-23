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

        public byte[] FromBase64String()
        {
            return Convert.FromBase64String(str);
        }
    }

    extension(byte[] data)
    {
        public string ToUTF8String()
        {
            return Encoding.UTF8.GetString(data);
        }

        public string FromUtf8Bytes()
        {
            return Encoding.UTF8.GetString(data);
        }

        public string ToBase64String()
        {
            return Convert.ToBase64String(data);
        }
    }
}
