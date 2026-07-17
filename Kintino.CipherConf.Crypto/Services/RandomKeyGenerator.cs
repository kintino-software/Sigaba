using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.Primitives;

namespace Kintino.CipherConf.Crypto.Services;

internal class RandomKeyGenerator : IRandomKeyGenerator
{
    PlainKey IRandomKeyGenerator.GenerateNewKey() => new(Algos.KeyGenerator.GenerateKey());
}
