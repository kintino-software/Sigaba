using Microsoft.Extensions.DependencyInjection;
using Sigaba.Crypto.Services.Ciphers;
using Sigaba.Crypto.Services.Ciphers.V1;

namespace Sigaba.Crypto.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddCryptoModule(this IServiceCollection services)
    {
        return services
            .AddSingleton<IVersionedCipher, CipherV1>()
            .AddSingleton<ICipher, Cipher>();

    }
}
