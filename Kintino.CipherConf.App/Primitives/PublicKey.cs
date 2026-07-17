using Kintino.CipherConf.App.Primitives;

namespace Kintino.CipherConf.App.Primitives;

public record PublicKey(Bytes Bytes)
{
    public override string ToString()
    {
        return Bytes.ToString();
    }
}
