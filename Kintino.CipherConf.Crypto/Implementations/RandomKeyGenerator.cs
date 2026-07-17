using Kintino.CipherConf.Crypto.Services.Algos;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Crypto.Implementations;

internal class RandomKeyGenerator : IRandomKeyGenerator
{
    PlainKey IRandomKeyGenerator.GenerateNewKey() => new(new PlainData(KeyGenerator.GenerateKey()));
}
