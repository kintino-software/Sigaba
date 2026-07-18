using Kintino.CipherConf.IO.Models;

namespace Kintino.CipherConf.IO.TestHelpers;

internal static class Extensions
{
    extension(ToolSettings)
    {
        public static ToolSettings FakeToolSettings()
        {
            return new()
            {
                FileRegex = ".*",
                PropertyRegex = ".*",
                Key = Convert.ToBase64String([1, 2, 3])
            };
        }
    }
}
