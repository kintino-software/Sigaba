using Microsoft.Extensions.FileSystemGlobbing;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Vipentti.IO.Abstractions.FileSystemGlobbing;

namespace Sigaba.App.Services.Settings;

internal partial class ToolSettingsV1(string fieldRegexPattern, string[] includeGlob, string[] excludeGlob)
{
    public record SerialObj
    {
        [JsonPropertyName("version")]
        public int Version { get; } = 1;
        [JsonPropertyName("fieldRegex")]
        public required string FieldRegex { get; init; } = string.Empty;
        [JsonPropertyName("include")]
        public required string[] IncludeFileGlob { get; init; } = [];
        [JsonPropertyName("exclude")]
        public required string[] ExcludeFileGlob { get; init; } = [];
    }

    private readonly Lazy<Regex> lazyFieldNameRegex = new(() => new Regex(fieldRegexPattern, RegexOptions.IgnoreCase));

}

internal partial class ToolSettingsV1 : IToolSettings<ToolSettingsV1>
{
    public int Version => 1;

    public static ToolSettingsV1 CreateDefault()
    {
        return new ToolSettingsV1(
            fieldRegexPattern: @"^.*_secret$",
            includeGlob: ["**/*.secrets.json"],
            excludeGlob: ["**node_modules/**", "**/bin/**", "**/obj/**"]);
    }

    public string Serialize()
    {
        return JsonSerializer.Serialize(new SerialObj
        {
            FieldRegex = fieldRegexPattern,
            IncludeFileGlob = includeGlob,
            ExcludeFileGlob = excludeGlob
        }, JsonHelper.JsonSerializerOptions);
    }

    public static ToolSettingsV1 Deserialize(string serialized)
    {
        var serialObj = JsonSerializer.Deserialize<SerialObj>(serialized, JsonHelper.JsonSerializerOptions)
            ?? throw new Exception("Failed to deserialize ToolSettingsV1.");
        if (serialObj.Version != 1)
            throw new Exception($"Unsupported version: {serialObj.Version}. Expected version: 1.");
        return new ToolSettingsV1(serialObj.FieldRegex, serialObj.IncludeFileGlob, serialObj.ExcludeFileGlob);
    }

    public bool FieldNamePredicate(string fieldName)
    {
        return this.lazyFieldNameRegex.Value.IsMatch(fieldName);
    }

    public IEnumerable<string> GetFilesWorkingSet(IFileSystem fs)
    {
        var cwd = fs.Directory.GetCurrentDirectory();
        var matcher = new Matcher();
        matcher.AddIncludePatterns(includeGlob);
        matcher.AddExcludePatterns(excludeGlob);
        return matcher.GetResultsInFullPath(fs, cwd);
    }
}
