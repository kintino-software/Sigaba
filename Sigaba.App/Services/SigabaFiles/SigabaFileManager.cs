using Sigaba.App.Services.Settings.V1;
using Sigaba.App.Services.SigabaFiles;
using Sigaba.Primitives;
using System.IO.Abstractions;

namespace Sigaba.App.Services.Settings;

internal partial class SigabaFileManager(IFileSystem fs) : ISigabaFileManager
{
    async Task ISigabaFileManager.SaveAsync(ISigabaFile sigabaFile, string filePath)
    {
        var content = sigabaFile switch
        {
            SigabaFileV1 v1 => v1.Serialize(),
            _ => throw new NotSupportedException($"ToolSettings version {sigabaFile.Version} is not supported.")
        };

        await fs.File.WriteAllTextAsync(filePath, content);
    }

    async Task<ISigabaFile> ISigabaFileManager.LoadAsync(string filePath)
    {
        var content = await fs.File.ReadAllTextAsync(filePath);
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
