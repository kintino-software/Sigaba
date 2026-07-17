using System.Security.Cryptography;

namespace Kintino.CipherConf.Crypto.Services.Algos;

internal static class RNG
{
    public static byte[] GetBytes(int size) => RandomNumberGenerator.GetBytes(size);
}
