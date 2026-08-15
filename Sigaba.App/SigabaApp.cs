using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.SigabaFiles;
using Sigaba.Crypto;
using Sigaba.Documents;
using Sigaba.Primitives;

namespace Sigaba.App;

internal class SigabaApp(
    ICipher cipher,
    ISigabaFileManager sigabaFileManager,
    IPrivateKeyManager privateKeyManager,
    IFileCipher fileCipher) : ISigabaApp
{
    async Task<InitializationResult> ISigabaApp.InitAsync(InitializationOptions options)
    {
        var (publicKey, privateKey) = cipher.GenerateKeys();

        var sigabaFile = sigabaFileManager.CreateDefault(publicKey);
        var privateKeyResult = await privateKeyManager.SaveAsync(privateKey, sigabaFile.ProjectId, options.PrivateKeyPassword);
        var sigabaFileResult = await sigabaFileManager.SaveAsync(sigabaFile, options.SigabaFileOutputDir);

        return new InitializationResult(sigabaFileResult.OutputPath, privateKeyResult.OupuptPath);
    }

    async Task<CipherResult> ISigabaApp.CipherFilesAsync(DirPath referenceFolderPath)
    {
        var (sigabaFile, sigabaFilePath) = await sigabaFileManager.LoadAsync(referenceFolderPath);

        List<string> affectedFiles = [];
        foreach (var filePath in sigabaFile.GetTargetFiles(sigabaFilePath.Parent()))
        {
            await fileCipher.CipherFile(filePath, sigabaFile.PublicKey, sigabaFile.FieldNamePredicate);
        }

        return new CipherResult(affectedFiles);
    }

    async Task<CipherResult> ISigabaApp.DecipherFilesAsync(DirPath referenceFolderPath, string password)
    {
        var (sigabaFile, sigabaFilePath) = await sigabaFileManager.LoadAsync(referenceFolderPath);
        var (privateKey, _) = await privateKeyManager.LoadAsync(sigabaFilePath.Parent(), sigabaFile.ProjectId, password);

        List<string> affectedFiles = [];
        foreach (var filePath in sigabaFile.GetTargetFiles(sigabaFilePath.Parent()))
        {
            await fileCipher.DecipherFile(filePath, privateKey);
        }

        return new CipherResult(affectedFiles);
    }

    async Task ISigabaApp.EditFileAsync(ITextEditor textEditor, FilePath editingFilePath)
    {
        var (sigabaFile, sigabaFilePath) = await sigabaFileManager.LoadAsync(editingFilePath.Parent());

        if (!sigabaFile.GetTargetFiles(sigabaFilePath.Parent()).Contains(editingFilePath))
            throw new InvalidOperationException(
                $"The file '{editingFilePath}' is not part of Sigaba target files. Make sure you have the correct filter in {Constants.SigabaFileName}.");

        await textEditor.EditFile(editingFilePath);
        await fileCipher.CipherFile(editingFilePath, sigabaFile.PublicKey, sigabaFile.FieldNamePredicate);
    }

}
