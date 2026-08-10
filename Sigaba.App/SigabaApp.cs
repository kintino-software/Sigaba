using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.SigabaFiles;
using Sigaba.Crypto;
using Sigaba.Documents;
using System.IO.Abstractions;

namespace Sigaba.App;

internal partial class SigabaApp(
    IFileSystem fs,
    ICipher cipher,
    ISigabaFileManager sigabaFileManager,
    IPrivateKeyManager privateKeyManager,
    IFileCipher fileCipher)
{
    private Task<ISigabaFile> GetNearestSigabaFile(string startingDir, out string sigabaFileDir)
    {
        var sigabaFilePath = fs.GetNearestFileWithNameGoingUp(startingDir, Constants.SigabaFileName)
            ?? throw new InvalidOperationException($"Could not find {Constants.SigabaFileName} in {startingDir} or any parent directory.");

        sigabaFileDir = fs.Path.GetDirectoryName(sigabaFilePath)
            ?? throw new InvalidOperationException($"Could not determine directory for {sigabaFilePath}.");

        return sigabaFileManager.LoadAsync(sigabaFilePath);
    }
}

internal partial class SigabaApp : ISigabaApp
{
    async Task ISigabaApp.InitAsync(InitializationOptions options)
    {
        var (publicKey, privateKey) = cipher.GenerateKeys();

        var sigabaFile = sigabaFileManager.CreateDefault(publicKey);

        await privateKeyManager.SaveAsync(sigabaFile.ProjectId, privateKey, options.PrivateKeyPassword);
        await sigabaFileManager.SaveAsync(sigabaFile, fs.Path.Combine(options.SigabaFileOutputDir, Constants.SigabaFileName));
    }

    async Task ISigabaApp.CipherFilesAsync(string referenceFolderPath)
    {
        var sigabaFile = await GetNearestSigabaFile(referenceFolderPath, out var sigabaFileDir);

        foreach (var filePath in sigabaFile.GetTargetFiles(fs, sigabaFileDir))
        {
            await fileCipher.CipherFile(filePath, sigabaFile.PublicKey, sigabaFile.FieldNamePredicate);
        }
    }

    async Task ISigabaApp.DecipherFilesAsync(string referenceFolderPath, string password)
    {
        var sigabaFile = await GetNearestSigabaFile(referenceFolderPath, out var sigabaFileDir);
        var privateKey = await privateKeyManager.LoadAsync(sigabaFile.ProjectId, password);

        foreach (var filePath in sigabaFile.GetTargetFiles(fs, sigabaFileDir))
        {
            await fileCipher.DecipherFile(filePath, privateKey);
        }
    }

    async Task ISigabaApp.EditFileAsync(ITextEditor textEditor, string editingFilePath)
    {
        var sigabaFile = await GetNearestSigabaFile(
            fs.Path.GetDirectoryName(editingFilePath)
                ?? throw new InvalidOperationException($"The file '{editingFilePath}' is not in a valid directory."),
            out var sigabaFileDir);

        if (!sigabaFile.GetTargetFiles(fs, sigabaFileDir).Contains(editingFilePath))
            throw new InvalidOperationException(
                $"The file '{editingFilePath}' is not part of Sigaba target files. Make sure you have the correct filter in {Constants.SigabaFileName}.");

        await textEditor.EditFile(editingFilePath);
        await fileCipher.CipherFile(editingFilePath, sigabaFile.PublicKey, sigabaFile.FieldNamePredicate);
    }

}
