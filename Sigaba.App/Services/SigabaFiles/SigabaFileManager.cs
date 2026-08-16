using Sigaba.App.Services.SigabaFiles.V1;
using Sigaba.Primitives;
using Sigaba.Primitives.Crypto;

namespace Sigaba.App.Services.SigabaFiles;

internal class SigabaFileManager : ISigabaFileManager
{
    async Task<SigabaFileSaveResult> ISigabaFileManager.SaveAsync(ISigabaFile sigabaFile, DirPath projectRoot)
    {
        var content = sigabaFile switch
        {
            SigabaFileV1 v1 => v1.Serialize(),
            _ => throw new NotSupportedException($"ToolSettings version {sigabaFile.Version} is not supported.")
        };

        var filePath = projectRoot.CombineAsFile(Constants.SigabaFileName);
        if (filePath.Exists)
            throw new InvalidOperationException($"File '{filePath}' already exists. Overwriting is not allowed.");

        await filePath.WriteAsync(content, overwrite: false); // not allowed to overwrite

        return new SigabaFileSaveResult(filePath);
    }

    async Task<SigabaFileLoadResult> ISigabaFileManager.LoadAsync(DirPath referenceFolder)
    {
        if (!referenceFolder.TryGetNearestFileWithNameGoingUp(Constants.SigabaFileName, out var sigabaFilePath))
            throw new InvalidOperationException($"Could not find '{Constants.SigabaFileName}' in '{referenceFolder}' or it's parents.");

        var content = await sigabaFilePath.ReadAsync();
        var version = JsonHelper.ReadVersionFromJson(content);
        ISigabaFile sigabaFile = version switch
        {
            1 => SigabaFileV1.Deserialize(content),
            _ => throw new NotSupportedException($"ToolSettings version {version} is not supported.")
        };

        return new SigabaFileLoadResult(sigabaFile, sigabaFilePath);
    }

    ISigabaFile ISigabaFileManager.CreateDefault(PublicKey publicKey)
    {
        var v1 = SigabaFileV1.CreateDefault(publicKey); // TODO: Consider querying the latest version of IToolSettings instead of hardcoding it.
        return v1;
    }

}
