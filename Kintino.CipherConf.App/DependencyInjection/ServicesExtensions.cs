using Kintino.CipherConf.App.Services.FileSystemServices;
using Kintino.CipherConf.App.Services.Serializers;
using Kintino.CipherConf.App.Services.Serializers.FileSerializers;
using Kintino.CipherConf.Crypto.DependencyInjection;
using Kintino.CipherConf.Documents.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;

namespace Kintino.CipherConf.App.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddApp(this IServiceCollection services, IFileSystem? fs = null)
    {
        services
            .AddCryptoModule()
            .AddDocumentsModule()
            .AddSingleton<IFileSystem>(fs ?? new FileSystem())
            .AddSingleton<IFsHelper, FsHelper>()
            .AddSingleton<IContextLoader, ContextLoader>()
            .AddSingleton<IToolSettingsFileSerializer, ToolSettingsFileSerializer>()
            .AddSingleton<IPublicKeyFileSerializer, PublicKeyFileSerializer>()
            .AddSingleton<IPrivateKeyFileSerializer, PrivateKeyFileSerializer>()
            .AddSingleton<FileContextHelper>()
            .AddSingleton<IEncryptConfigApp, EncryptConfigApp>();

        return services;
    }
}
