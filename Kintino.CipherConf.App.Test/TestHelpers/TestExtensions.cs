using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.Models;
using Kintino.CipherConf.App.Primitives;
using System.Security.Cryptography;

namespace Kintino.CipherConf.App.TestHelpers;

public static class TestExtensions
{
    private static byte[] GenerateRandomBytes() => RandomNumberGenerator.GetBytes(8);

    extension(IAsymmetricCipher asymmetricCipher)
    {
        public void MockKeysGeneration(PublicKey generatedPublicKey, PrivateKey generatedPrivateKey = null)
        {
            asymmetricCipher
            .WhenForAnyArgs(x => x.CreateNewKeyPair(out Arg.Any<PublicKey>(), out Arg.Any<PrivateKey>()))
            .Do(x =>
            {
                x[0] = generatedPublicKey;
                x[1] = generatedPrivateKey;
            });
        }
    }

    extension(Bytes)
    {
        public static Bytes FakeBytes() => new(GenerateRandomBytes());
    }

    extension(CryptoBytes)
    {
        public static CryptoBytes FakeCryptoBytes() => new(Bytes.FakeBytes());
    }

    extension(CryptoKey)
    {
        public static CryptoKey FakeCryptoKey() => new(Bytes.FakeBytes());
    }

    extension(Nonce)
    {
        public static Nonce FakeNonce() => new(Bytes.FakeBytes());
    }

    extension(PlainBytes)
    {
        public static PlainBytes FakePlainBytes() => new(Bytes.FakeBytes());
    }

    extension(PlainKey)
    {
        public static PlainKey FakePlainKey() => new(Bytes.FakeBytes());
    }

    extension(PrivateKey)
    {
        public static PrivateKey FakePrivateKey() => new(Bytes.FakeBytes());
    }

    extension(PublicKey)
    {
        public static PublicKey FakePublicKey() => new(Bytes.FakeBytes());
    }

    extension(String64)
    {
        public static String64 FakeString64() => new(Convert.ToBase64String(Bytes.FakeBytes()));
    }

    extension(Context)
    {
        public static Context FakeContext() => new()
        {
            FileRegex = new(@".*"),
            PropertyRegex = new(@"_secret$"),
            Key = CryptoKey.FakeCryptoKey(),
            PrivateKey = PrivateKey.FakePrivateKey(),
            PublicKey = PublicKey.FakePublicKey()
        };
    }
}
