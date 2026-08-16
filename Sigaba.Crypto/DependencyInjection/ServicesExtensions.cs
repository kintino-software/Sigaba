using Microsoft.Extensions.DependencyInjection;
using Sigaba.Crypto.Services.Ciphers;
using Sigaba.Crypto.Services.Ciphers.V1;

namespace Sigaba.Crypto.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddCryptoModule(this IServiceCollection services)
    {
        return services
            // cipher versions here
            .AddSingleton<IVersionedCipher, CipherV1>()
            // other services here
            .AddSingleton<ICipher, Cipher>();

    }
}
