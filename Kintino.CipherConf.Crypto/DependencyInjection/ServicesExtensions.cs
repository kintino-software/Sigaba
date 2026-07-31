using Kintino.CipherConf.Crypto.Services.Ciphers;
using Kintino.CipherConf.Crypto.Services.Ciphers.V1;
using Microsoft.Extensions.DependencyInjection;

namespace Kintino.CipherConf.Crypto.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddCryptoModule(this IServiceCollection services)
    {
        return services
            .AddSingleton<IVersionedAsymmetricCipher, AsymmetricCipherV1>()
            .AddSingleton<IVersionedSymmetricCipher, SymmetricCipherV1>()
            .AddSingleton<ISymmetricCipher, SymmetricCipher>()
            .AddSingleton<IAsymmetricCipher, AsymmetricCipher>();

    }
}
