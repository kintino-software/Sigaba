namespace Kintino.CipherConf.Crypto;

public interface ICipherFactory
{
    IAsymmetricCipher GetLatestAsymmetricCipher();
    ISymmetricCipher GetLatestSymmetricCipher();
    ISymmetricCipher GetSymmetricCipher(int version);
    IAsymmetricCipher GetAsymmetricCipher(int version);
}
