using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.Primitives;

namespace Kintino.CipherConf.Crypto.Services;

internal class NonceGenerator : INonceGenerator
{
    Nonce INonceGenerator.NewNonce() => new(Algos.NonceGenerator.GenerateNonce());
}
