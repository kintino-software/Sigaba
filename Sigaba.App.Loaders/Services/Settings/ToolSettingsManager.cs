using System.IO.Abstractions;

namespace Sigaba.App.Services.Settings;

internal partial class ToolSettingsManager(IFileSystem fs)
{
    private string Cwd => fs.Directory.GetCurrentDirectory();
    private string FilePath => fs.Path.Combine(Cwd, Constants.ToolSettingsFileName);

    private bool FileExists() => fs.File.Exists(FilePath);
}

internal partial class ToolSettingsManager : IToolSettingsManager
{
    Task<bool> IToolSettingsManager.ExistsAsync() => Task.FromResult(FileExists());

    async Task IToolSettingsManager.SaveDefaultAsync()
    {
        var v1 = ToolSettingsV1.CreateDefault(); // TODO: Consider querying the latest version of IToolSettings instead of hardcoding it.
        await fs.File.WriteAllTextAsync(FilePath, v1.Serialize());
    }

    async Task<IToolSettings> IToolSettingsManager.LoadAsync()
    {
        if (!FileExists())
            throw new InvalidOperationException("ToolSettings file does not exist.");

        var content = await fs.File.ReadAllTextAsync(FilePath);

        var version = JsonHelper.ReadVersionFromJson(content);
        return version switch
        {
            1 => ToolSettingsV1.Deserialize(content),
            _ => throw new NotSupportedException($"ToolSettings version {version} is not supported.")
        };
    }
}
