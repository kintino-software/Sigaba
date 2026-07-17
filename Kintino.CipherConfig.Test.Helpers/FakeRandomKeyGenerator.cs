using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConfig;

public class FakeRandomKeyGenerator : IRandomKeyGenerator
{
    public PlainKey GenerateNewKey() => PlainKey.FakePlainKey();
}
