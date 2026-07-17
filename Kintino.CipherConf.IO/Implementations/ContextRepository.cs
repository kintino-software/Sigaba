using Kintino.CipherConf.IO.Dependencies;
using Kintino.CipherConf.IO.Primitives;
using Kintino.CipherConf.IO.Services;
using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;
using System.IO.Abstractions;

namespace Kintino.CipherConf.IO.Implementations;

internal class ContextRepository(
    IFileSystem fs,
    IIOConfiguration config,
    IDataSerializer serializer) : IContextRepository
{
    async ValueTask IContextRepository.SaveContext(IContext context, string folderPath)
    {
        if (TryGetFiles(folderPath, out var privateKeyFilePath, out var publicKeyFilePath, out var configFilePath))
        {
            throw new InvalidOperationException($"Project already initialized in folder '{folderPath}'.");
        }

        var publicKeyContent = serializer.SerializePublicKey(context.PublicKey);
        var privateKeyContent = serializer.SerializePrivateKey(context.PrivateKey);
        var settingsContent = serializer.SerializeToolSettings(new ToolSettings
        {
            PropertyRegex = "", // TEMP
            FileRegex = "", // TEMP
            Key = context.Key.Bytes.ToBase64String(),
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

    async ValueTask<IContext> IContextRepository.GetContext(string folderPath)
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

        return new ConcreteContext()
        {
            PublicKey = publicKey,
            PrivateKey = privateKey,
            FieldFilter = new RegexFieldFilter(toolSettings.PropertyRegex),
            FileFilter = new RegexFileFilter(toolSettings.FileRegex),
            Key = new EncryptedKey(new EncryptedData(toolSettings.Key.FromBase64String()))
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
