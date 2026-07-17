using Kintino.CipherConf.App.Models;
using Kintino.CipherConf.App.Primitives;
using Kintino.CipherConf.IO.Primitives;

namespace Kintino.CipherConf.IO.TestHelpers;

internal static class Extensions
{
    extension(InitData)
    {
        public static InitData FakeInitData()
        {
            return new()
            {
                FileRegex = ".*",
                FolderPath = "TestFolder",
                Key = new CryptoKey(new byte[] { 1, 2, 3, 4 }),
                PrivateKey = new PrivateKey(new byte[] { 5, 6, 7, 8 }),
                PropertyRegex = ".*",
                PublicKey = new PublicKey(new byte[] { 9, 10, 11, 12 })
            };
        }
    }

    extension(PublicKey)
    {
        public static PublicKey FakePublicKey()
        {
            return new(new byte[] { 9, 10, 11, 12 });
        }
    }

    extension(PrivateKey)
    {
        public static PrivateKey FakePrivateKey()
        {
            return new(new byte[] { 5, 6, 7, 8 });
        }
    }

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
