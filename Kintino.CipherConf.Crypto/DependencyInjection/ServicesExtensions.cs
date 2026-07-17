using Kintino.CipherConf.Crypto.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Kintino.CipherConf.Crypto.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddECCrypto(this IServiceCollection services)
    {
        return services
            .AddSingleton<IAsymmetricCipher, AsymmetricCipher>()
            .AddSingleton<INonceGenerator, NonceGenerator>()
            .AddSingleton<IRandomKeyGenerator, RandomKeyGenerator>()
            .AddSingleton<ISymmetricCipher, SymmetricCipher>();

    }
}
