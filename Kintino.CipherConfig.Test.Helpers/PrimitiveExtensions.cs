using Kintino.CipherConf.Primitives;
using System.Security.Cryptography;

namespace Kintino.CipherConfig;

public static class PrimitiveExtensions
{
    extension(EncryptedData)
    {
        public static EncryptedData FakeEncryptedData(int size = 8) => new(RandomNumberGenerator.GetBytes(size));
    }
    extension(PlainData)
    {
        public static PlainData FakePlainData(int size = 8) => new(RandomNumberGenerator.GetBytes(size));
    }
    extension(EncryptedKey)
    {
        public static EncryptedKey FakeEncryptedKey() => new(EncryptedData.FakeEncryptedData());
    }
    extension(Nonce)
    {
        public static Nonce FakeNonce() => new(PlainData.FakePlainData());
    }
    extension(PlainKey)
    {
        public static PlainKey FakePlainKey() => new(PlainData.FakePlainData());
    }
    extension(PrivateKey)
    {
        public static PrivateKey FakePrivateKey() => new(PlainData.FakePlainData());
    }
    extension(PublicKey)
    {
        public static PublicKey FakePublicKey() => new(PlainData.FakePlainData());
    }
}
