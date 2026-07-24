using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents;
using Kintino.CipherConf.IO;

namespace Kintino.CipherConf.App.Services;

public class EncryptConfigApp(
    IFileOperations fileOperations,
    IAsymmetricCipher asymmetricCipher,
    IContextFactory contextFactory,
    IContextRepository contextRepository,
    IFileCipher fileCipher)
    : IEncryptConfigApp
{
    async ValueTask IEncryptConfigApp.Init(string targetFolder)
    {
        if (await contextRepository.HasContext(targetFolder))
        {
            throw new InvalidOperationException($"The folder '{targetFolder}' is already initialized.");
        }
        var (publicKey, privateKey) = asymmetricCipher.CreateNewKeyPair();
        var context = contextFactory.CreateDefault(publicKey, privateKey);
        await contextRepository.SaveContext(context, targetFolder);
    }

    async ValueTask IEncryptConfigApp.CipherFiles(string targetFolder)
    {
        var context = await contextRepository.GetContext(targetFolder)
            ?? throw new InvalidOperationException("Could not retrieve context. Try to initialize the folder first.");
        var publicKey = context.PublicKey
            ?? throw new InvalidOperationException("You cannot cipher files without a public key.");


        var filesToEncrypt = await fileOperations.GetFilesFromDirectory(targetFolder, context.FileFilter);
        foreach (var filePath in filesToEncrypt)
        {
            await fileCipher.CipherFile(filePath, publicKey, context.FieldFilter);
        }
    }

    async ValueTask IEncryptConfigApp.DecipherFiles(string targetFolder)
    {
        var context = await contextRepository.GetContext(targetFolder)
            ?? throw new InvalidOperationException("Could not retrieve context. Try to initialize the folder first.");
        var privateKey = context.PrivateKey
            ?? throw new InvalidOperationException("You cannot decipher files without a private key.");


        var filesToEncrypt = await fileOperations.GetFilesFromDirectory(targetFolder, context.FileFilter);
        foreach (var filePath in filesToEncrypt)
        {
            await fileCipher.DecipherFile(filePath, privateKey);
        }
    }

    async ValueTask IEncryptConfigApp.EditFile(ITextEditor textEditor, string targetFolder, string editingFilePath)
    {
        var context = await contextRepository.GetContext(targetFolder)
            ?? throw new InvalidOperationException("Could not retrieve context. Try to initialize the folder first.");
        if (context.PublicKey == null)
            throw new InvalidOperationException("You cannot edit files without a public key.");

        await fileOperations.WithTempFile(editingFilePath, async (tempFilePath) =>
        {
            await fileOperations.CopyWithOverwrite(editingFilePath, tempFilePath);
            await textEditor.EditFile(tempFilePath);
        }, async (tempFilePath) =>
        {
            await fileCipher.CipherFile(tempFilePath, context.PublicKey, context.FieldFilter);
            await fileOperations.CopyWithOverwrite(tempFilePath, editingFilePath);
        });
    }

}
