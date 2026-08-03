namespace Sigaba.Crypto.Services.Ciphers;

internal interface IVersionedAsymmetricCipher : IAsymmetricCipher
{
    byte Version { get; }
}
