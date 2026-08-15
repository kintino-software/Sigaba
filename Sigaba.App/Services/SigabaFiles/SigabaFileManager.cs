using Sigaba.App.Services.SigabaFiles.V1;
using Sigaba.Primitives;

namespace Sigaba.App.Services.SigabaFiles;

internal partial class SigabaFileManager : ISigabaFileManager
{
    async Task ISigabaFileManager.SaveAsync(ISigabaFile sigabaFile, FilePath filePath)
    {
        var content = sigabaFile switch
        {
            SigabaFileV1 v1 => v1.Serialize(),
            _ => throw new NotSupportedException($"ToolSettings version {sigabaFile.Version} is not supported.")
        };

        await filePath.WriteAsync(content, overwrite: false); // not allowed to overwrite
    }

    async Task<ISigabaFile?> ISigabaFileManager.LoadAsync(FilePath filePath)
    {
        if (!filePath.Exists)
            return null;

        var content = await filePath.ReadAsync();
        var version = JsonHelper.ReadVersionFromJson(content);
        return version switch
        {
            1 => SigabaFileV1.Deserialize(content),
            _ => throw new NotSupportedException($"ToolSettings version {version} is not supported.")
        };
    }

    ISigabaFile ISigabaFileManager.CreateDefault(PublicKey publicKey)
    {
        var v1 = SigabaFileV1.CreateDefault(publicKey); // TODO: Consider querying the latest version of IToolSettings instead of hardcoding it.
        return v1;
    }

}
