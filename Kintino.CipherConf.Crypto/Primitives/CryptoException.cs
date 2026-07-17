namespace Kintino.CipherConf.Crypto.Primitives;

public class CryptoException : Exception
{
    public CryptoException(string message) : base(message) { }
    public CryptoException(string message, Exception innerException) : base(message, innerException) { }
}
