using Kintino.CipherConf.Crypto.Services.Algos;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Crypto.Implementations;

internal class NonceGenerator : INonceGenerator
{
    Nonce INonceGenerator.NewNonce() => new(new PlainData(RandomNonceGenerator.GenerateNonce()));
}
