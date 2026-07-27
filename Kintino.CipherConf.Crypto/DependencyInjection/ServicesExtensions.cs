using Kintino.CipherConf.Crypto.Implementations;
using Kintino.CipherConf.Crypto.Services.Ciphers.V1;
using Microsoft.Extensions.DependencyInjection;

namespace Kintino.CipherConf.Crypto.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddCryptoModule(this IServiceCollection services)
    {
        return services
            .AddSingleton<IAsymmetricCipher, AsymmetricCipherV1>()
            .AddSingleton<ISymmetricCipher, SymmetricCipherV1>()
            .AddSingleton<ICipherFactory, CipherFactory>();

    }
}
