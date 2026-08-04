namespace Sigaba.Crypto.Services.Ciphers;

internal interface IVersionedCipher : ICipher
{
    byte Version { get; }
}
