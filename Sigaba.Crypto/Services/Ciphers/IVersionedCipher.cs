using Sigaba.Crypto.Services.Ciphers.V1;

namespace Sigaba.Crypto.Services.Ciphers;

internal interface IVersionedCipher : ICipher
{
    byte Version { get; }
}
