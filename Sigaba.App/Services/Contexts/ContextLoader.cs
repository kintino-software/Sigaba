using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.SigabaFiles;
using Sigaba.Primitives;
using System.IO.Abstractions;

namespace Sigaba.App.Services.Contexts;

internal partial class ContextLoader(IFileSystem fs, ISigabaFileManager sigabaFileManager, IPrivateKeyManager privateKeyManager)
{
    private string? GetNearestFileWithName(string startDirectory, string fileName)
    {
        var currentDirectory = startDirectory;
        while (!fs.File.Exists(fs.Path.Combine(currentDirectory, fileName)))
        {
            currentDirectory = fs.Path.GetDirectoryName(currentDirectory);
            if (string.IsNullOrEmpty(currentDirectory))
                return null;
        }
        return currentDirectory;
    }
}

internal partial class ContextLoader : IContextLoader
{
    async Task IContextLoader.CreateContextAsync(string initializationFolderPath, PublicKey publicKey, PrivateKey privateKey)
    {
        var sigabaFile = sigabaFileManager.CreateDefault(publicKey);
        await sigabaFileManager.SaveAsync(sigabaFile, fs.Path.Combine(initializationFolderPath, Constants.SigabaFileName));
        await privateKeyManager.SaveAsync(privateKey);
    }

    async Task<Context> IContextLoader.LoadContextFromFolderAsync(string folderPath)
    {
        var sigabaFilePath = GetNearestFileWithName(folderPath, Constants.SigabaFileName)
            ?? throw new FileNotFoundException($"Could not find {Constants.SigabaFileName} in {folderPath} or any parent directory.");
        var rootFolder = fs.Path.GetDirectoryName(sigabaFilePath)
            ?? throw new InvalidOperationException($"Could not determine root folder for {sigabaFilePath}.");

        var sigabaFile = await sigabaFileManager.LoadAsync(sigabaFilePath);
        var privateKey = await privateKeyManager.LoadAsync();

        var context = new Context
        {
            SigabaRootDir = rootFolder,
            SigabaFilePath = sigabaFilePath,
            PublicKey = sigabaFile.PublicKey,
            PrivateKey = privateKey,
            FieldFilterPredicate = sigabaFile.FieldNamePredicate,
            WorkingSetFiles = sigabaFile.GetTargetFiles(fs, rootFolder)
        };

        return context;
    }
}