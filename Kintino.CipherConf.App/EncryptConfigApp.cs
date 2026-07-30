using Kintino.CipherConf.App.Models;
using Kintino.CipherConf.App.Services.FileSystemServices;
using Kintino.CipherConf.App.Services.Serializers;
using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents;

namespace Kintino.CipherConf.App;

internal class EncryptConfigApp(
    IAsymmetricCipher asymmetricCipher,
    IFsHelper fsHelper,
    IFileCipher fileCipher,
    IContextLoader contextLoader)
    : IEncryptConfigApp
{
    async Task IEncryptConfigApp.InitAsync()
    {
        if (await contextLoader.HasContextAsync())
            throw new InvalidOperationException($"The app is already initialized.");
        var (publicKey, privateKey) = asymmetricCipher.CreateNewKeyPair();
        await contextLoader.CreateContextAsync(publicKey, privateKey);
    }

    async Task IEncryptConfigApp.CipherFilesAsync()
    {
        var context = await GetContextOrThrow();
        if (context.PublicKey == null)
            throw new InvalidOperationException("Missing public key in context. You cannot cipher files without a public key.");

        foreach (var filePath in fsHelper.Crawl(context.AppContextDirectory, context.IncludeFileGlob, context.ExcludeFileGlob))
        {
            await fileCipher.CipherFile(filePath, context.PublicKey, context.FieldRegex.IsMatch);

        }
    }

    async Task IEncryptConfigApp.DecipherFilesAsync()
    {
        var context = await GetContextOrThrow();
        if (context.PrivateKey == null)
            throw new InvalidOperationException("Missing private key in context. You cannot decipher files without a private key.");

        foreach (var filePath in fsHelper.Crawl(context.AppContextDirectory, context.IncludeFileGlob, context.ExcludeFileGlob))
        {
            await fileCipher.DecipherFile(filePath, context.PrivateKey);
        }
    }

    async Task IEncryptConfigApp.EditFileAsync(ITextEditor textEditor, string editingFilePath)
    {
        var context = await GetContextOrThrow();
        if (context.PublicKey == null)
            throw new InvalidOperationException("You cannot edit files without a public key.");

        await fsHelper.WithTempFileAsync(
            originalFile: editingFilePath,
            editingOperation: async (tempFilePath) =>
            {
                await fsHelper.CopyAndOverwrite(editingFilePath, tempFilePath);
                await textEditor.EditFile(tempFilePath);
            },
            beforeDeleteOperation: async (tempFilePath) =>
            {
                await fileCipher.CipherFile(tempFilePath, context.PublicKey, context.FieldRegex.IsMatch);
                await fsHelper.CopyAndOverwrite(tempFilePath, editingFilePath);
            });
    }

    // helpers

    private async Task<Context> GetContextOrThrow()
    {
        var context = await contextLoader.LoadContextAsync();
        return context ?? throw new InvalidOperationException("Could not retrieve context. Try to initialize the folder first.");
    }

}
