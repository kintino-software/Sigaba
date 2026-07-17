using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConfig;

public class FakeNonceGenerator : INonceGenerator
{
    public Nonce NewNonce() => Nonce.FakeNonce();
}
