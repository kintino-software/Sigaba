using System.Security.Cryptography;

namespace Sigaba.Crypto.TestHelpers;

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
