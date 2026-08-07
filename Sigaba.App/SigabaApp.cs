using Sigaba.App.Services.Contexts;
using Sigaba.Crypto;
using Sigaba.Documents;
using System.IO.Abstractions;

namespace Sigaba.App;

internal partial class SigabaApp(
    IFileSystem fs,
    ICipher cipher,
    IContextLoader contextLoader,
    IFileCipher fileCipher) : ISigabaApp
{
    async Task ISigabaApp.InitAsync(string initializationFolderPath)
    {
        var (publicKey, privateKey) = cipher.GenerateKeys();
        await contextLoader.CreateContextAsync(initializationFolderPath, publicKey, privateKey);
    }

    async Task ISigabaApp.CipherFilesAsync(string referenceFolderPath)
    {
        var context = await contextLoader.LoadContextFromFolderAsync(referenceFolderPath);

        foreach (var filePath in context.WorkingSetFiles)
        {
            await fileCipher.CipherFile(filePath, context.PublicKey, context.FieldFilterPredicate);
        }
    }

    async Task ISigabaApp.DecipherFilesAsync(string referenceFolderPath)
    {
        var context = await contextLoader.LoadContextFromFolderAsync(referenceFolderPath);

        var privateKey = context.PrivateKey
            ?? throw new InvalidOperationException("You cannot decipher files without a private key.");

        foreach (var filePath in context.WorkingSetFiles)
        {
            await fileCipher.DecipherFile(filePath, privateKey);
        }
    }

    async Task ISigabaApp.EditFileAsync(ITextEditor textEditor, string editingFilePath)
    {
        var directoryPath = fs.Path.GetDirectoryName(editingFilePath)
            ?? throw new InvalidOperationException($"The file '{editingFilePath}' is not in a valid directory.");
        var context = await contextLoader.LoadContextFromFolderAsync(directoryPath);

        if (!context.WorkingSetFiles.Contains(editingFilePath))
            throw new InvalidOperationException($"The file '{editingFilePath}' is not part of the working set. You cannot edit it.");

        await textEditor.EditFile(editingFilePath);
        await fileCipher.CipherFile(editingFilePath, context.PublicKey, context.FieldFilterPredicate);
    }

}
