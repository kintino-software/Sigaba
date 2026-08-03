using Kintino.CipherConf.App.Services.Settings;
using Microsoft.Extensions.FileSystemGlobbing;
using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Vipentti.IO.Abstractions.FileSystemGlobbing;

namespace Kintino.CipherConf.App.Services.Settings;

public class ToolSettings : IToolSettings
{
    private static readonly JsonSerializerOptions serializerOptions = new() { PropertyNamingPolicy = null, WriteIndented = true };
    private readonly Lazy<Predicate<string>> lazyFieldFilter;
    private readonly Lazy<IEnumerable<string>> lazyWorkingSetFiles;

    // IToolSettings implementation

    Predicate<string> IToolSettings.FieldFilter { get => this.lazyFieldFilter.Value; }
    IEnumerable<string> IToolSettings.WorkingSetFiles { get => this.lazyWorkingSetFiles.Value; }

    // initialization

    private ToolSettings(Lazy<Predicate<string>> lazyFieldFilter, Lazy<IEnumerable<string>> lazyWorkingSetFiles)
    {
        this.lazyFieldFilter = lazyFieldFilter;
        this.lazyWorkingSetFiles = lazyWorkingSetFiles;
    }

    public static ToolSettings CreateFromSerialized(string serializedSettings, string currentFolder, IFileSystem fs)
    {
        var version = ResolveVersion(serializedSettings);
        return version switch
        {
            1 => InitiliazeWithV1(serializedSettings, fs, currentFolder),
            _ => throw new InvalidOperationException($"Unsupported settings version: {version}.")
        };
    }

    private static ToolSettings InitiliazeWithV1(string serializedSettings, IFileSystem fs, string currentFolder)
    {
        var v1 = JsonSerializer.Deserialize<SchemaV1>(serializedSettings, serializerOptions)
                ?? throw new InvalidOperationException("Failed to deserialize settings for version 1.");
        var lazyFieldFilter = new Lazy<Predicate<string>>(() => new Regex(v1.FieldRegex, RegexOptions.IgnoreCase).IsMatch);
        var lazyWorkingSetFiles = new Lazy<IEnumerable<string>>(() =>
        {
            var matcher = new Matcher();
            matcher.AddIncludePatterns(v1.IncludeFileGlob);
            matcher.AddExcludePatterns(v1.ExcludeFileGlob);
            return matcher.GetResultsInFullPath(fs, currentFolder);
        });
        return new ToolSettings(lazyFieldFilter, lazyWorkingSetFiles);
    }

    private static int ResolveVersion(string jsonDocument)
    {
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonDocument), isFinalBlock: true, state: default);
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case (JsonTokenType.PropertyName):
                    if (reader.GetString() == "version")
                    {
                        reader.Read();
                        return reader.GetInt32();
                    }
                    break;
            }
        }
        return 0;
    }

    // Serialization

    public static string SerializeDefault()
    {
        var latest = new SchemaV1()
        {
            FieldRegex = @"^.*_secret$",
            IncludeFileGlob = ["**/*_secrets.json"],
            ExcludeFileGlob = ["**node_modules/**", "**/bin/**", "**/obj/**"]
        };
        return JsonSerializer.Serialize(latest, serializerOptions);
    }
}
