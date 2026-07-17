namespace Kintino.CipherConf.IO.Adaptors;

internal static class PrimitiveExtensions
{
    extension(byte[] bytes)
    {
        public string ToBase64String()
        {
            return Convert.ToBase64String(bytes);
        }
    }

    extension(string str)
    {
        public byte[] FromBase64String()
        {
            return Convert.FromBase64String(str);
        }
    }

}
