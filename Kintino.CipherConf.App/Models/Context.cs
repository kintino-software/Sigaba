using Kintino.CipherConf.Primitives;
using System.Text.RegularExpressions;

namespace Kintino.CipherConf.App.Models;

internal class Context
{
    public int SettingsVersion { get; init; }
    public required Regex FieldRegex { get; init; }
    public required string[] IncludeFileGlob { get; init; }
    public required string[] ExcludeFileGlob { get; init; }
    public required PrivateKey? PrivateKey { get; init; }
    public required PublicKey? PublicKey { get; init; }
    public required string AppContextDirectory { get; init; }
}

