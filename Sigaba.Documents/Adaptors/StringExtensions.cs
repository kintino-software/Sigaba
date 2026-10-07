namespace Sigaba.Documents.Adaptors;

internal static class StringExtensions
{
    extension(string s)
    {
        public string AsLF()
        {
            return s.Replace("\r\n", "\n");
        }
    }
}
