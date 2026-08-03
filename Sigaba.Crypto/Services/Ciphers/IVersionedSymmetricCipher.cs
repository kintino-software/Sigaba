namespace Sigaba.Crypto.Services.Ciphers;

internal interface IVersionedSymmetricCipher : ISymmetricCipher
{
    byte Version { get; }
}
