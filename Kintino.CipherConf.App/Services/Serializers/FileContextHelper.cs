using System.IO.Abstractions;

namespace Kintino.CipherConf.App.Services.Serializers;

public class FileContextHelper(IFileSystem fs)
{
    private readonly Lazy<string> settingsFolderPath = new(() => GetSettingsFolderPath(fs));
    public readonly static string SettingsFileName = "cipherconf.settings.json";
    public string SettingsFolderPath => settingsFolderPath.Value;

    private static string GetSettingsFolderPath(IFileSystem fs)
    {
        var currentDirectory = fs.Directory.GetCurrentDirectory();
        while (!fs.File.Exists(fs.Path.Combine(currentDirectory, SettingsFileName)))
        {
            currentDirectory = fs.Path.GetDirectoryName(currentDirectory);
            if (string.IsNullOrEmpty(currentDirectory))
                throw new InvalidOperationException("Could not find the settings folder.");
        }
        return currentDirectory;
    }
}
