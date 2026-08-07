namespace Sigaba.App;

public static class Constants
{
    public static string SigabaFileName { get; } = "sigaba.json";
    public static string PrivateKeyFileName { get; } = "private.key";
    public static string ToolSystemFolderName { get; } = ".sigaba";
    public static string SigabaSystemFolderPath { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ToolSystemFolderName);
}
