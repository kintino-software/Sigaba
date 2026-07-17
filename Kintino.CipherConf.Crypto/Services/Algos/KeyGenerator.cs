using Kintino.CipherConf.Crypto.Services.Algos;
namespace Kintino.CipherConf.Crypto.Services.Algos;

internal static class KeyGenerator
{
    public const int KeySize = 32; // 256 bits

    public static byte[] GenerateKey()
    {
        return RNG.GetBytes(KeySize);
    }
}
