using Microsoft.Extensions.FileSystemGlobbing;
using Sigaba.Primitives;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.RegularExpressions;
using Vipentti.IO.Abstractions.FileSystemGlobbing;

namespace Sigaba.App.Services.SigabaFiles.V1;

internal partial class SigabaFileV1(
    string fieldRegexPattern,
    string[] includeGlob,
    string[] excludeGlob,
    PublicKey publicKey)
{
    private readonly Lazy<Regex> lazyFieldNameRegex = new(() => new Regex(fieldRegexPattern, RegexOptions.IgnoreCase));
    private PublicKey currentPublicKey = publicKey;

    public string Serialize()
    {
        var schema = new SchemaV1
        {
            Configuration = new SchemaV1.ConfigurationSchema
            {
                FieldRegex = fieldRegexPattern,
                IncludeFileGlob = includeGlob,
                ExcludeFileGlob = excludeGlob
            },
            PublicKeyBase64 = currentPublicKey.ToBase64(),
        };

        return JsonSerializer.Serialize(schema, JsonHelper.JsonSerializerOptions);
    }

    public static SigabaFileV1 CreateDefault(PublicKey publicKey)
    {
        return new SigabaFileV1(
            fieldRegexPattern: @"^.*_secret$",
            includeGlob: ["**/*.secrets.json"],
            excludeGlob: ["**node_modules/**", "**/bin/**", "**/obj/**"],
            publicKey: publicKey);
    }

    public static SigabaFileV1 Deserialize(string serialized)
    {
        var schema = JsonSerializer.Deserialize<SchemaV1>(serialized, JsonHelper.JsonSerializerOptions)
            ?? throw new Exception("Failed to deserialize ToolSettingsV1.");

        if (schema.Version != 1)
            throw new Exception($"Unsupported version: {schema.Version}. Expected version: 1.");

        return new SigabaFileV1(
            schema.Configuration.FieldRegex,
            schema.Configuration.IncludeFileGlob,
            schema.Configuration.ExcludeFileGlob,
            PublicKey.FromBase64(schema.PublicKeyBase64));
    }
}

internal partial class SigabaFileV1 : ISigabaFile
{
    int ISigabaFile.Version { get; } = 1;

    PublicKey ISigabaFile.PublicKey { get => this.currentPublicKey; set => this.currentPublicKey = value; }

    bool ISigabaFile.FieldNamePredicate(string name)
    {
        return this.lazyFieldNameRegex.Value.IsMatch(name);
    }

    IEnumerable<string> ISigabaFile.GetTargetFiles(IFileSystem fs, string rootFolder)
    {
        var cwd = rootFolder;
        var matcher = new Matcher();
        matcher.AddIncludePatterns(includeGlob);
        matcher.AddExcludePatterns(excludeGlob);
        return matcher.GetResultsInFullPath(fs, cwd);
    }

}

