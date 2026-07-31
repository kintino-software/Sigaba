namespace Kintino.CipherConf.Crypto.Services.Ciphers;

internal interface IVersionedSymmetricCipher : ISymmetricCipher
{
    byte Version { get; }
}
