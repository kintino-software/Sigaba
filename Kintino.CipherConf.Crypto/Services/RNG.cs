using System.Security.Cryptography;

namespace Kintino.CipherConf.Crypto.Services;

internal static class RNG
{
    public static byte[] GetBytes(int sizeInBytes) => RandomNumberGenerator.GetBytes(sizeInBytes);
}
