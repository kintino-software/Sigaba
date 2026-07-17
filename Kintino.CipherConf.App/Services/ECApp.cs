using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.Models;

namespace Kintino.CipherConf.App.Services;

public class ECApp(
    IFileOperations fileOperations,
    ITextEditor textEditor,
    IAsymmetricCipher asymmetricCipher,
    ISymmetricCipher symmetricCipher,
    IContextRepository contextRepository,
    IFileCipher fileCipher,
    IFacade facade) : IECApp
{
    async ValueTask IECApp.Init(string targetFolder)
    {
        if (await contextRepository.HasContext(targetFolder))
        {
            throw new InvalidOperationException($"The folder '{targetFolder}' is already initialized.");
        }

        var (publicKey, privateKey, cryptoKey) = facade.CreateContextKeys();

        var initData = new InitData()
        {
            FileRegex = @"^appsettings.*\.json$",
            PropertyRegex = @"_secret$",
            FolderPath = targetFolder,
            PrivateKey = privateKey,
            PublicKey = publicKey,
            Key = cryptoKey,
        };
        await contextRepository.CreateContext(initData, targetFolder);
    }

    async ValueTask IECApp.CipherFiles(string targetFolder)
    {
        var context = await contextRepository.GetContext(targetFolder)
            ?? throw new InvalidOperationException();

        var plainKey = facade.DecryptKeyFromContext(context);
        var filesToEncrypt = await fileOperations.GetFilesFromDirectory(targetFolder, context.FileRegex);
        foreach (var filePath in filesToEncrypt)
        {
            await fileCipher.CipherFile(filePath, plainKey, symmetricCipher, context.PropertyRegex);
        }
    }

    async ValueTask IECApp.DecipherFiles(string targetFolder)
    {
        var context = await contextRepository.GetContext(targetFolder)
            ?? throw new InvalidOperationException();

        var plainKey = facade.DecryptKeyFromContext(context);
        var filesToEncrypt = await fileOperations.GetFilesFromDirectory(targetFolder, context.FileRegex);
        foreach (var filePath in filesToEncrypt)
        {
            await fileCipher.DecipherFile(filePath, plainKey, symmetricCipher);
        }
    }

    async ValueTask IECApp.EditFile(string targetFolder, string editingFilePath)
    {
        var context = await contextRepository.GetContext(targetFolder)
            ?? throw new InvalidOperationException();

        var plainKey = context.DecryptKey(asymmetricCipher);

        await fileOperations.WithTempFile(editingFilePath, async (tempFilePath) =>
        {
            await fileOperations.CopyWithOverwrite(editingFilePath, tempFilePath);
            await textEditor.EditFile(tempFilePath);
            await fileCipher.DecipherFile(tempFilePath, plainKey, symmetricCipher);
        }, async (tempFilePath) =>
        {
            await fileCipher.CipherFile(tempFilePath, plainKey, symmetricCipher, context.PropertyRegex);
            await fileOperations.CopyWithOverwrite(tempFilePath, editingFilePath);
        });
    }

}
