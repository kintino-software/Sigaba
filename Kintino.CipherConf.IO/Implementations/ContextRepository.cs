using Kintino.CipherConf.IO.Dependencies;
using Kintino.CipherConf.Models;
using System.IO.Abstractions;

namespace Kintino.CipherConf.IO.Implementations;

internal class ContextRepository(IFileSystem fs, IIOConfiguration config) : IContextRepository
{
    async ValueTask IContextRepository.SaveContext(IContext context, string folderPath)
    {
        if (TryGetFiles(folderPath, out var privateKeyFilePath, out var publicKeyFilePath, out var configFilePath))
        {
            throw new InvalidOperationException($"Project already initialized in folder '{folderPath}'.");
        }
        if (context is not ConcreteContext concreteContext)
        {
            throw new InvalidOperationException($"Context must be of type '{nameof(ConcreteContext)}'.");
        }
        var serialization = concreteContext.Serialize();
        try
        {

            fs.File.WriteAllText(privateKeyFilePath, serialization.PrivateKeyStr);
            fs.File.WriteAllText(publicKeyFilePath, serialization.PublicKeyStr);
            fs.File.WriteAllText(configFilePath, serialization.SettingsStr);
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
        var publicKeyStr = await fs.File.ReadAllTextAsync(publicKeyFilePath);
        var privateKeyStr = await fs.File.ReadAllTextAsync(privateKeyFilePath);
        var settingsStr = await fs.File.ReadAllTextAsync(configFilePath);

        return ConcreteContext.Deserialize(privateKeyStr, publicKeyStr, settingsStr);
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
