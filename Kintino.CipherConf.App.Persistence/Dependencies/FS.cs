using System.IO.Abstractions;

namespace Kintino.CipherConf.App.Dependencies;

internal static class FS
{
    private static IFileSystem current = new FileSystem();
    public static IFileSystem Current => current;

    public static void Setup(IFileSystem fileSystem) => current = fileSystem;
}
