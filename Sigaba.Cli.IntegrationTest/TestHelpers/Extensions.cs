using System.IO.Abstractions;

namespace Sigaba.Cli.TestHelpers;

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
