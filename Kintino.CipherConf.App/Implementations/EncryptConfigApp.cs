using Kintino.CipherConf.App.Services;
using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents;
using Kintino.CipherConf.IO;
using Kintino.CipherConf.Tooling;

namespace Kintino.CipherConf.App.Implementations;

public class EncryptConfigApp(
    IFileOperations fileOperations,
    ITextEditor textEditor,
    ISymmetricCipher symmetricCipher,
    IContextFactory contextFactory,
    IContextRepository contextRepository,
    IFileCipher fileCipher,
    IFacade facade) : IEncryptConfigApp
{
    async ValueTask IEncryptConfigApp.Init(string targetFolder)
    {
        if (await contextRepository.HasContext(targetFolder))
        {
            throw new InvalidOperationException($"The folder '{targetFolder}' is already initialized.");
        }
        var (publicKey, privateKey, encryptedKey) = facade.CreateContextKeys();
        var context = contextFactory.CreateDefault(publicKey, privateKey, encryptedKey);
        await contextRepository.SaveContext(context, targetFolder);
    }

    async ValueTask IEncryptConfigApp.CipherFiles(string targetFolder)
    {
        var context = await contextRepository.GetContext(targetFolder)
            ?? throw new InvalidOperationException();

        var plainKey = facade.DecryptKeyFromContext(context);
        var filesToEncrypt = await fileOperations.GetFilesFromDirectory(targetFolder, context.FileFilter);
        foreach (var filePath in filesToEncrypt)
        {
            await fileCipher.CipherFile(filePath, plainKey, symmetricCipher, context.FieldFilter);
        }
    }

    async ValueTask IEncryptConfigApp.DecipherFiles(string targetFolder)
    {
        var context = await contextRepository.GetContext(targetFolder)
            ?? throw new InvalidOperationException();

        var plainKey = facade.DecryptKeyFromContext(context);
        var filesToEncrypt = await fileOperations.GetFilesFromDirectory(targetFolder, context.FileFilter);
        foreach (var filePath in filesToEncrypt)
        {
            await fileCipher.DecipherFile(filePath, plainKey, symmetricCipher);
        }
    }

    async ValueTask IEncryptConfigApp.EditFile(string targetFolder, string editingFilePath)
    {
        var context = await contextRepository.GetContext(targetFolder)
            ?? throw new InvalidOperationException();

        var plainKey = facade.DecryptKeyFromContext(context);

        await fileOperations.WithTempFile(editingFilePath, async (tempFilePath) =>
        {
            await fileOperations.CopyWithOverwrite(editingFilePath, tempFilePath);
            await textEditor.EditFile(tempFilePath);
            await fileCipher.DecipherFile(tempFilePath, plainKey, symmetricCipher);
        }, async (tempFilePath) =>
        {
            await fileCipher.CipherFile(tempFilePath, plainKey, symmetricCipher, context.FieldFilter);
            await fileOperations.CopyWithOverwrite(tempFilePath, editingFilePath);
        });
    }

}
