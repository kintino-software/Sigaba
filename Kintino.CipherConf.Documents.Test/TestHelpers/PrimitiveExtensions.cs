using Kintino.CipherConf.App.Primitives;
using Kintino.CipherConf.Documents.Adaptors;

namespace Kintino.CipherConf.Documents.TestHelpers;

internal static class PrimitiveExtensions
{
    extension(PlainBytes)
    {
        public static PlainBytes FakePlainBytes() => new("plain".ToUTF8Bytes());
    }

    extension(CryptoBytes)
    {
        public static CryptoBytes FakeCryptoBytes() => new("encrypted".ToUTF8Bytes());
    }

    extension(PlainKey)
    {
        public static PlainKey FakePlainKey() => new("plain-key".ToUTF8Bytes());
    }

    extension(Nonce)
    {
        public static Nonce FakeNonce() => new("nonce".ToUTF8Bytes());
    }
}
