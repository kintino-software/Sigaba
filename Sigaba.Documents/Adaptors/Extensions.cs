using System.Text;

namespace Sigaba.Documents.Adaptors;

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

    extension(byte[] bytes)
    {
        public string ToUTF8String()
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public string FromUtf8Bytes()
        {
            return Encoding.UTF8.GetString(bytes);
        }

    }
}
