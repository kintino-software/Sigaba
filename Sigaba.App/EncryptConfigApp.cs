using Sigaba.App.Service;
using Sigaba.Documents;

namespace Sigaba.App;

internal class EncryptConfigApp(
    IFsHelper fsHelper,
    IContextLoader contextLoader,
    IFileCipher fileCipher) : IEncryptConfigApp
{
    Task IEncryptConfigApp.InitAsync()
    {
        return contextLoader.CreateContextAsync();
    }

    async Task IEncryptConfigApp.CipherFilesAsync()
    {
        var context = await GetContextOrThrow();
        var publicKey = context.GetPublicKey()
            ?? throw new InvalidOperationException("Missing public key in context. You cannot cipher files without a public key.");

        foreach (var filePath in context.GetWorkingSetFiles())
        {
            await fileCipher.CipherFile(filePath, publicKey, context.FieldNameFilter);
        }
    }

    async Task IEncryptConfigApp.DecipherFilesAsync()
    {
        var context = await GetContextOrThrow();
        var privateKey = context.GetPrivateKey()
            ?? throw new InvalidOperationException("Missing private key in context. You cannot decipher files without a private key.");

        foreach (var filePath in context.GetWorkingSetFiles())
        {
            await fileCipher.DecipherFile(filePath, privateKey);
        }
    }

    async Task IEncryptConfigApp.EditFileAsync(ITextEditor textEditor, string editingFilePath)
    {
        var context = await GetContextOrThrow();
        var publicKey = context.GetPublicKey()
            ?? throw new InvalidOperationException("You cannot edit files without a public key.");

        await fsHelper.WithTempFileAsync(
            originalFile: editingFilePath,
            editingOperation: async (tempFilePath) =>
            {
                await fsHelper.CopyAndOverwrite(editingFilePath, tempFilePath);
                await textEditor.EditFile(tempFilePath);
            },
            beforeDeleteOperation: async (tempFilePath) =>
            {
                await fileCipher.CipherFile(tempFilePath, publicKey, context.FieldNameFilter);
                await fsHelper.CopyAndOverwrite(tempFilePath, editingFilePath);
            });
    }

    // helpers

    private async Task<IContext> GetContextOrThrow()
    {
        var context = await contextLoader.LoadContextAsync();
        return context ?? throw new InvalidOperationException("Could not retrieve context. Try to initialize the folder first.");
    }


}
