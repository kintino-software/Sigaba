using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.SigabaFiles;
using Sigaba.Crypto;
using Sigaba.Documents;
using Sigaba.Primitives;
using Sigaba.Services;
using System.IO.Abstractions;

namespace Sigaba.App;

internal partial class SigabaApp(
    IFileSystem fs,
    IEnvironmentVariables env,
    ICipher cipher,
    ISigabaFileManager sigabaFileManager,
    IPrivateKeyManager privateKeyManager,
    IFileCipher fileCipher)
{
    private async Task<(ISigabaFile, DirPath)> GetNearestSigabaFile(DirPath startingDir)
    {
        if (!startingDir.TryGetNearestFileWithNameGoingUp(Constants.SigabaFileName, out var filePath))
            throw new InvalidOperationException($"Could not find {Constants.SigabaFileName} in {startingDir} or any parent directory.");

        var sigabaFile = await sigabaFileManager.LoadAsync(filePath)
            ?? throw new InvalidOperationException("Could not load Sigaba file. It may be corrupted or invalid.");

        return (sigabaFile, filePath.Parent());
    }

    private FilePath GetDefaultPrivateKeyOutputPath(string projectId)
    {
        return fs.NewFilePath(Constants.SigabaSystemDir, projectId, Constants.PrivateKeyFileName);
    }

    private IEnumerable<FilePath> GetPrivateKeyPossibleLocations(DirPath projectDirPath, string projectId)
    {
        // by order of precedence:

        // #1. Get from environment variable
        var envVar = env.GetEnvironmentVariable(Constants.PrivateKeyDirEnvVarKey);
        if (envVar != null)
        {
            yield return fs.NewFilePath(envVar, Constants.PrivateKeyFileName);
        }

        // #2. Get from project directory
        yield return projectDirPath.CombineAsFile(Constants.PrivateKeyFileName);

        // #3. Get from default system directory
        yield return GetDefaultPrivateKeyOutputPath(projectId);
    }

    private async Task<PrivateKey> ResolvePrivateKey(DirPath projectDirPath, string projectId, string password)
    {
        foreach (var possibleLocation in GetPrivateKeyPossibleLocations(projectDirPath, projectId))
        {
            if (possibleLocation.Exists)
            {
                return await privateKeyManager.LoadAsync(possibleLocation, password)
                    ?? throw new InvalidOperationException(
                        $"Could not load private key from {possibleLocation}. It may be corrupted or invalid.");
            }
        }
        throw new InvalidOperationException(
            $"Could not find private key for project '{projectId}'. Please ensure the private key is available in one of the expected locations.");
    }
}

internal partial class SigabaApp : ISigabaApp
{
    async Task<InitializationResult> ISigabaApp.InitAsync(InitializationOptions options)
    {
        var (publicKey, privateKey) = cipher.GenerateKeys();

        var sigabaFile = sigabaFileManager.CreateDefault(publicKey);
        var sigabaFilePath = options.SigabaFileOutputDir.CombineAsFile(Constants.SigabaFileName);
        var privateKeyPath = GetDefaultPrivateKeyOutputPath(sigabaFile.ProjectId);

        await privateKeyManager.SaveAsync(privateKey, privateKeyPath, options.PrivateKeyPassword);
        await sigabaFileManager.SaveAsync(sigabaFile, sigabaFilePath);

        return new InitializationResult(sigabaFilePath, privateKeyPath);
    }

    async Task<CipherResult> ISigabaApp.CipherFilesAsync(DirPath referenceFolderPath)
    {
        var (sigabaFile, dir) = await GetNearestSigabaFile(referenceFolderPath);

        List<string> affectedFiles = [];
        foreach (var filePath in sigabaFile.GetTargetFiles(dir))
        {
            await fileCipher.CipherFile(filePath, sigabaFile.PublicKey, sigabaFile.FieldNamePredicate);
        }

        return new CipherResult(affectedFiles);
    }

    async Task<CipherResult> ISigabaApp.DecipherFilesAsync(DirPath referenceFolderPath, string password)
    {
        var (sigabaFile, dir) = await GetNearestSigabaFile(referenceFolderPath);
        var privateKey = await ResolvePrivateKey(dir, sigabaFile.ProjectId, password);

        List<string> affectedFiles = [];
        foreach (var filePath in sigabaFile.GetTargetFiles(dir))
        {
            await fileCipher.DecipherFile(filePath, privateKey);
        }

        return new CipherResult(affectedFiles);
    }

    async Task ISigabaApp.EditFileAsync(ITextEditor textEditor, FilePath editingFilePath)
    {
        var (sigabaFile, dir) = await GetNearestSigabaFile(editingFilePath.Parent());

        if (!sigabaFile.GetTargetFiles(dir).Contains(editingFilePath))
            throw new InvalidOperationException(
                $"The file '{editingFilePath}' is not part of Sigaba target files. Make sure you have the correct filter in {Constants.SigabaFileName}.");

        await textEditor.EditFile(editingFilePath);
        await fileCipher.CipherFile(editingFilePath, sigabaFile.PublicKey, sigabaFile.FieldNamePredicate);
    }

}
