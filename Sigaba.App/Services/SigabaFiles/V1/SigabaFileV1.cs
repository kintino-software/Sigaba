using Microsoft.Extensions.FileSystemGlobbing;
using Sigaba.Primitives.Crypto;
using Sigaba.Primitives.FileSystem;
using System.Text.Json;
using System.Text.RegularExpressions;
using Vipentti.IO.Abstractions.FileSystemGlobbing;

namespace Sigaba.App.Services.SigabaFiles.V1;

internal partial class SigabaFileV1(
    string fieldRegexPattern,
    string[] includeGlob,
    string[] excludeGlob,
    string projectId,
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
            Meta = new SchemaV1.MetaSchema
            {
                ProjectId = projectId,
                PublicKeyBase64 = currentPublicKey.ToBase64()
            }
        };

        return JsonSerializer.Serialize(schema, JsonHelper.JsonSerializerOptions);
    }

    public static SigabaFileV1 CreateDefault(PublicKey publicKey)
    {
        return new SigabaFileV1(
            projectId: Guid.NewGuid().ToString("N"),
            publicKey: publicKey,
            fieldRegexPattern: @"^.*_secret$",
            includeGlob: ["**/*.secrets.json"],
            excludeGlob: ["**node_modules/**", "**/bin/**", "**/obj/**"]);
    }

    public static SigabaFileV1 Deserialize(string serialized)
    {
        var schema = JsonSerializer.Deserialize<SchemaV1>(serialized, JsonHelper.JsonSerializerOptions)
            ?? throw new Exception("Failed to deserialize ToolSettingsV1.");

        if (schema.Meta.Version != 1)
            throw new Exception($"Unsupported version: {schema.Meta.Version}. Expected version: 1.");

        return new SigabaFileV1(
            fieldRegexPattern: schema.Configuration.FieldRegex,
            includeGlob: schema.Configuration.IncludeFileGlob,
            excludeGlob: schema.Configuration.ExcludeFileGlob,
            projectId: schema.Meta.ProjectId,
            publicKey: PublicKey.FromBase64(schema.Meta.PublicKeyBase64));
    }
}

internal partial class SigabaFileV1 : ISigabaFile
{
    int ISigabaFile.Version { get; } = 1;

    string ISigabaFile.ProjectId { get => projectId; }

    PublicKey ISigabaFile.PublicKey { get => this.currentPublicKey; set => this.currentPublicKey = value; }

    bool ISigabaFile.FieldNamePredicate(string name)
    {
        return this.lazyFieldNameRegex.Value.IsMatch(name);
    }

    IEnumerable<FilePath> ISigabaFile.GetTargetFiles(DirPath rootFolder)
    {
        var matcher = new Matcher();
        matcher.AddIncludePatterns(includeGlob);
        matcher.AddExcludePatterns(excludeGlob);
        var matches = matcher.GetResultsInFullPath(rootFolder.Fs, rootFolder.AbsolutePath);
        return matches.Select(f => rootFolder.Fs.NewFilePath(f));
    }

}

