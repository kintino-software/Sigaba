using System.Security.Cryptography;

namespace Kintino.CipherConf.Crypto.Services;

internal static class RNG
{
    public static byte[] GetBytes(int sizeInBytes)
    {
        if (sizeInBytes < 1)
            throw new ArgumentException("Size must be greater than 0.", nameof(sizeInBytes));
        return RandomNumberGenerator.GetBytes(sizeInBytes);
    }
}
