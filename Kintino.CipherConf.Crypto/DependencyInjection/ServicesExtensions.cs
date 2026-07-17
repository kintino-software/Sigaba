using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.Crypto.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kintino.CipherConf.Crypto.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddECCrypto(this IServiceCollection services)
    {
        return services
            .AddSingleton<ISymmetricCipher, SymmetricCipher>()
            .AddSingleton<IAsymmetricCipher, AsymmetricCipher>()
            .AddSingleton<IRandomKeyGenerator, RandomKeyGenerator>()
            .AddSingleton<INonceGenerator, NonceGenerator>();

    }
}
