using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents;
using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConfig;

public class FakeFileCipher : IFileCipher
{
    private bool shouldThrow = false;

    public ValueTask CipherFile(string filePath, PlainKey plainKey, ISymmetricCipher symmetricCipher, IFieldFilter fieldFilter)
    {
        if (shouldThrow)
        {
            throw new InvalidOperationException("Ciphering failed.");
        }
        return ValueTask.CompletedTask;
    }

    public ValueTask DecipherFile(string filePath, PlainKey plainKey, ISymmetricCipher symmetricCipher)
    {
        if (shouldThrow)
        {
            throw new InvalidOperationException("Deciphering failed.");
        }
        return ValueTask.CompletedTask;
    }

    public FakeFileCipher MakeItThrow(bool shouldThrow = true)
    {
        this.shouldThrow = shouldThrow;
        return this;
    }
}
