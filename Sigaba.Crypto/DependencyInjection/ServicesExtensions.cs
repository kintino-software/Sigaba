using Microsoft.Extensions.DependencyInjection;
using Sigaba.Crypto.Services.Ciphers;
using Sigaba.Crypto.Services.Ciphers.V1;

namespace Sigaba.Crypto.DependencyInjection;

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
