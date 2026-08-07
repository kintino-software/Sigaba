using System.Text;
using System.Text.Json;

namespace Sigaba.App.Services.SigabaFiles;

/// <summary>
/// Common helpers for JSON serialization and deserialization.
/// </summary>
internal static class JsonHelper
{
    /// <summary>
    /// Reads the version number from a JSON document string. 
    /// It looks for a property named "version" (case-insensitive) and returns its integer value. 
    /// If the property is not found, it returns -1.
    /// </summary>
    /// <param name="jsonDocument">The JSON document string to read the version from.</param>
    /// <returns>The version number if found; otherwise, -1.</returns>
    public static int ReadVersionFromJson(string jsonDocument)
    {
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonDocument), isFinalBlock: true, state: default);
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case (JsonTokenType.PropertyName):
                    if (reader.GetString()?.Equals("version", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        reader.Read();
                        return reader.GetInt32();
                    }
                    break;
            }
        }
        return -1;
    }

    /// <summary>
    /// Gets the default JSON serializer options used for serialization and deserialization of tool settings.
    /// </summary>
    public static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions()
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true,
        IndentSize = 2,
    };
}
