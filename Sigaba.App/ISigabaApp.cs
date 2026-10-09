using Sigaba.App.Dependencies;
using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.SigabaFiles;
using Sigaba.Crypto;
using Sigaba.Documents;
using Sigaba.Primitives.Crypto;
using Sigaba.Primitives.FileSystem;
using System.IO.Abstractions;

namespace Sigaba.App;

public interface ISigabaApp
{
    Task<InitializationResult> InitAsync(InitializationOptions options);
    Task<CipherResult> CipherFilesAsync(DirPath referenceFolderPath);
    Task<CipherResult> DecipherFilesAsync(DirPath referenceFolderPath, string password);
    Task<EditFileResult> EditFileAsync(ITextEditor textEditor, FilePath editingFilePath);
}


internal class SigabaApp(
    ICipher cipher,
    IFileSystem fs,
    ISigabaFileManager sigabaFileManager,
    IPrivateKeyManager privateKeyManager,
    IFileCipher fileCipher) : ISigabaApp
{
    async Task<InitializationResult> ISigabaApp.InitAsync(InitializationOptions options)
    {
        var (_, _, sigabaFilePath, privateKeyPath) =
            await InitAsyncCore(options.SigabaFileOutputDir, options.PrivateKeyPassword);

        return new InitializationResult(sigabaFilePath, privateKeyPath);

    }

    async Task<CipherResult> ISigabaApp.CipherFilesAsync(DirPath referenceFolderPath)
    {
        var (sigabaFile, sigabaFilePath) = await sigabaFileManager.LoadAsync(referenceFolderPath);
        var topMostFolder = sigabaFilePath.GetContainingDirectory();
        var affectedFiles = await CipherFilesAsyncCore(
            sigabaFile.PublicKey,
            sigabaFile.FieldNamePredicate,
            sigabaFile.GetTargetFiles(fs, topMostFolder));
        return new CipherResult(affectedFiles);
    }

    async Task<CipherResult> ISigabaApp.DecipherFilesAsync(DirPath referenceFolderPath, string password)
    {
        var (sigabaFile, sigabaFilePath) = await sigabaFileManager.LoadAsync(referenceFolderPath);
        var projectRoot = sigabaFilePath.GetContainingDirectory();
        var (privateKey, _) = await privateKeyManager.LoadAsync(projectRoot, sigabaFile.ProjectId, password);

        var affectedFiles = await DecipherFilesAsyncCore(
            privateKey,
            sigabaFile.GetTargetFiles(fs, projectRoot));

        return new CipherResult(affectedFiles);
    }

    async Task<EditFileResult> ISigabaApp.EditFileAsync(ITextEditor textEditor, FilePath editingFilePath)
    {
        if (!fs.File.Exists(editingFilePath))
            throw new FileNotFoundException($"The file '{editingFilePath}' does not exist.");

        var referenceDir = editingFilePath.GetContainingDirectory();

        var (sigabaFile, sigabaFilePath) = await sigabaFileManager.LoadAsync(referenceDir);

        // Check if the file is part of the target files in the Sigaba file
        if (!sigabaFile.IsTargetFile(fs, editingFilePath, referenceDir))
            throw new InvalidOperationException(
                $"The file '{editingFilePath}' is not part of Sigaba target files. Make sure you have the correct filter in {Constants.SigabaFileName}.");


        await EditFileAsyncCore(textEditor, editingFilePath, sigabaFile.PublicKey, sigabaFile.FieldNamePredicate);

        return new EditFileResult(editingFilePath);
    }

    // helpers

    private async Task<(PublicKey publicKey, PrivateKey privateKey, FilePath sigabaFilePath, FilePath privateKeyPath)> InitAsyncCore(DirPath sigabaFileOutputDir, string privateKeyPassword)
    {
        var (publicKey, privateKey) = cipher.GenerateKeys();

        var sigabaFile = sigabaFileManager.CreateDefault(publicKey);

        var privateKeyResult = await privateKeyManager.SaveAsync(privateKey, sigabaFile.ProjectId, privateKeyPassword);
        var sigabaFileResult = await sigabaFileManager.SaveAsync(sigabaFile, sigabaFileOutputDir);

        return (publicKey, privateKey, sigabaFileResult.OutputPath, privateKeyResult.OutputPath);
    }


    private async Task<IEnumerable<FilePath>> CipherFilesAsyncCore(
        PublicKey publicKey,
        Predicate<string> fieldFilter,
        IEnumerable<FilePath> filesToCipher)
    {
        List<FilePath> affectedFiles = [];
        foreach (var filePath in filesToCipher)
        {
            await fileCipher.CipherFile(filePath, publicKey, fieldFilter);
            affectedFiles.Add(filePath.ToString());
        }

        return affectedFiles;
    }

    private async Task<IEnumerable<FilePath>> DecipherFilesAsyncCore(PrivateKey privateKey, IEnumerable<FilePath> filesToDecipher)
    {
        List<FilePath> affectedFiles = [];
        foreach (var filePath in filesToDecipher)
        {
            await fileCipher.DecipherFile(filePath, privateKey);
            affectedFiles.Add(filePath);
        }

        return affectedFiles;
    }

    private async Task EditFileAsyncCore(
        ITextEditor textEditor,
        FilePath editingFilePath,
        PublicKey publicKey,
        Predicate<string> fieldFilter)
    {
        await textEditor.EditFile(editingFilePath);
        await fileCipher.CipherFile(editingFilePath, publicKey, fieldFilter);
    }

}
