using System.Security.Cryptography;

namespace Kintino.CipherConf.Crypto.TestHelpers;

public abstract class BaseTest
{
    protected byte[] GetRandomBytes(int length = 32)
    {
        var randomBytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return randomBytes;
    }

}
