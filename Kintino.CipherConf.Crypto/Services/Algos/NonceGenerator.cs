namespace Kintino.CipherConf.Crypto.Services.Algos;

internal static class NonceGenerator
{
    public const int NonceSize = 12; // 96 bits
    public static byte[] GenerateNonce()
    {
        return RNG.GetBytes(NonceSize);
    }
}
