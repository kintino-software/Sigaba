using System.IO.Abstractions;

namespace Kintino.CipherConf.Cli.TestHelpers;

public static class Extensions
{
    extension(IFileSystem fs)
    {
        public JsonDoc InspectJson(string filePath)
        {
            var jsonContent = fs.File.ReadAllText(filePath);
            return JsonDoc.Parse(jsonContent);
        }
    }
}
