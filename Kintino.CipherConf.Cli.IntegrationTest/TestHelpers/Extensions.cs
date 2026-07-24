using System.IO.Abstractions;

namespace Kintino.CipherConf.Cli.TestHelpers;

public static class Extensions
{
    extension(IFileSystem fs)
    {
        public JsonTester InspectJson(string filePath)
        {
            var jsonContent = fs.File.ReadAllText(filePath);
            return JsonTester.Parse(jsonContent);
        }
    }
}
