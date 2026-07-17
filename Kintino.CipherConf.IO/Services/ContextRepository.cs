using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.Models;
using Kintino.CipherConf.App.Primitives;
using Kintino.CipherConf.IO.Dependencies;
using Kintino.CipherConf.IO.Primitives;
using System.IO.Abstractions;
using System.Text.RegularExpressions;

namespace Kintino.CipherConf.IO.Services;

internal class ContextRepository(
    IFileSystem fs,
    IIOConfiguration config,
    IDataSerializer serializer) : IContextRepository
{
    async ValueTask IContextRepository.CreateContext(InitData initData, string folderPath)
    {
        if (TryGetFiles(folderPath, out var privateKeyFilePath, out var publicKeyFilePath, out var configFilePath))
        {
            throw new InvalidOperationException($"Project already initialized in folder '{folderPath}'.");
        }
        var publicKeyContent = serializer.SerializePublicKey(initData.PublicKey);
        var privateKeyContent = serializer.SerializePrivateKey(initData.PrivateKey);
        var settingsContent = serializer.SerializeToolSettings(new ToolSettings
        {
            PropertyRegex = initData.PropertyRegex,
            FileRegex = initData.FileRegex,
            Key = initData.Key.Bytes.AsBase64().Value
        });
        try
        {
            fs.File.WriteAllText(privateKeyFilePath, privateKeyContent);
            fs.File.WriteAllText(publicKeyFilePath, publicKeyContent);
            fs.File.WriteAllText(configFilePath, settingsContent);
        }
        catch
        {
            // If any of the file writes fail, we want to clean up any files that were created to avoid leaving a partially initialized context.
            SafeDeleteFiles(privateKeyFilePath, publicKeyFilePath, configFilePath);
            throw;
        }
    }

    async ValueTask<Context> IContextRepository.GetContext(string folderPath)
    {
        if (!TryGetFiles(folderPath, out var privateKeyFilePath, out var publicKeyFilePath, out var configFilePath))
        {
            throw new InvalidOperationException($"Project not initialized in folder '{folderPath}'.");
        }
        var publicKeyContent = await fs.File.ReadAllTextAsync(publicKeyFilePath);
        var privateKeyContent = await fs.File.ReadAllTextAsync(privateKeyFilePath);
        var settingsContent = await fs.File.ReadAllTextAsync(configFilePath);

        var publicKey = serializer.DeserializePublicKey(publicKeyContent);
        var privateKey = serializer.DeserializePrivateKey(privateKeyContent);
        var toolSettings = serializer.DeserializeToolSettings(settingsContent);

        return new Context()
        {
            PublicKey = publicKey,
            PrivateKey = privateKey,
            PropertyRegex = toolSettings.PropertyRegex == null ? null : new Regex(toolSettings.PropertyRegex),
            FileRegex = toolSettings.FileRegex == null ? null : new Regex(toolSettings.FileRegex),
            Key = new CryptoKey(new String64(toolSettings.Key).AsBytes())
        };
    }

    ValueTask<bool> IContextRepository.HasContext(string folderPath)
    {
        return ValueTask.FromResult(TryGetFiles(folderPath, out _, out _, out _));
    }

    // helpers

    private bool TryGetFiles(string folderPath, out string privateKeyFilePath, out string publicKeyFilePath, out string toolSettingsFileName)
    {
        privateKeyFilePath = fs.Path.Combine(folderPath, config.PrivateKeyFileName);
        publicKeyFilePath = fs.Path.Combine(folderPath, config.PublicKeyFileName);
        toolSettingsFileName = fs.Path.Combine(folderPath, config.ToolSettingsFileName);
        return fs.File.Exists(privateKeyFilePath) ||
               fs.File.Exists(publicKeyFilePath) ||
               fs.File.Exists(toolSettingsFileName);
    }

    private void SafeDeleteFiles(params string[] files)
    {
        foreach (string file in files)
        {
            if (fs.File.Exists(file))
            {
                fs.File.Delete(file);
            }
        }
    }
}
