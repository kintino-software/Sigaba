using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.SigabaFiles;
using Sigaba.Crypto;
using Sigaba.Documents;
using Sigaba.Primitives;

namespace Sigaba.App;

internal partial class SigabaApp(
    ICipher cipher,
    ISigabaFileManager sigabaFileManager,
    IPrivateKeyManager privateKeyManager,
    IFileCipher fileCipher)
{
    private Task<ISigabaFile> GetNearestSigabaFile(DirPath startingDir, out FilePath sigabaFilePath)
    {
        if (!startingDir.TryGetNearestFileWithNameGoingUp(Constants.SigabaFileName, out var filePath))
            throw new InvalidOperationException($"Could not find {Constants.SigabaFileName} in {startingDir} or any parent directory.");

        sigabaFilePath = filePath;
        return sigabaFileManager.LoadAsync(filePath);
    }
}

internal partial class SigabaApp : ISigabaApp
{
    async Task ISigabaApp.InitAsync(InitializationOptions options)
    {
        var (publicKey, privateKey) = cipher.GenerateKeys();

        var sigabaFile = sigabaFileManager.CreateDefault(publicKey);

        await privateKeyManager.SaveAsync(sigabaFile.ProjectId, privateKey, options.PrivateKeyPassword, options.SigabaFileOutputDir);
        await sigabaFileManager.SaveAsync(sigabaFile, options.SigabaFileOutputDir.CombineAsFile(Constants.SigabaFileName));
    }

    async Task ISigabaApp.CipherFilesAsync(DirPath referenceFolderPath)
    {
        var sigabaFile = await GetNearestSigabaFile(referenceFolderPath, out var sigabaFilePath);

        foreach (var filePath in sigabaFile.GetTargetFiles(sigabaFilePath.Parent()))
        {
            await fileCipher.CipherFile(filePath, sigabaFile.PublicKey, sigabaFile.FieldNamePredicate);
        }
    }

    async Task ISigabaApp.DecipherFilesAsync(DirPath referenceFolderPath, string password, DirPath? privateKeyPreferedLocation)
    {
        var sigabaFile = await GetNearestSigabaFile(referenceFolderPath, out var sigabaFilePath);
        var privateKey = await privateKeyManager.LoadAsync(sigabaFile.ProjectId, password, privateKeyPreferedLocation);

        foreach (var filePath in sigabaFile.GetTargetFiles(sigabaFilePath.Parent()))
        {
            await fileCipher.DecipherFile(filePath, privateKey);
        }
    }

    async Task ISigabaApp.EditFileAsync(ITextEditor textEditor, FilePath editingFilePath)
    {
        var sigabaFile = await GetNearestSigabaFile(editingFilePath.Parent(), out var sigabaFilePath);

        if (!sigabaFile.GetTargetFiles(sigabaFilePath.Parent()).Contains(editingFilePath))
            throw new InvalidOperationException(
                $"The file '{editingFilePath}' is not part of Sigaba target files. Make sure you have the correct filter in {Constants.SigabaFileName}.");

        await textEditor.EditFile(editingFilePath);
        await fileCipher.CipherFile(editingFilePath, sigabaFile.PublicKey, sigabaFile.FieldNamePredicate);
    }

}
