using Sigaba.Documents.Services.Env.Parser.Superpower;

namespace Sigaba.Documents.Services.Env.Parser;

/// <summary>
/// Based on this spec: https://github.com/env-lang/env/blob/main/env.md
/// </summary>
internal static class EnvParser
{
    public static IReadOnlyDictionary<string, EnvEntry> Parse(string envDocument)
    {
        var dic = new Dictionary<string, EnvEntry>(StringComparer.OrdinalIgnoreCase);
        HashSet<TokenType> valueTokenTypes =
        [
            TokenType.Value,
            TokenType.MultiLineValue,
            TokenType.LiteralValue,
            TokenType.EscapedLiteralValue
        ];

        var tokens = EnvTokenizer.Tokenize(envDocument).ToArray();
        for (int i = 0; i < tokens.Length; i++)
        {
            var keyToken = tokens[i];
            if (keyToken.Kind == TokenType.Key)
            {
                if (i >= tokens.Length - 1)
                {
                    throw new InvalidOperationException($"Key '{keyToken.ToStringValue()}' is not followed by a value.");
                }
                var valueToken = tokens[i + 1];
                if (!valueTokenTypes.Contains(valueToken.Kind))
                {
                    throw new InvalidOperationException($"Key '{keyToken.ToStringValue()}' is not followed by a valid value token. Found: {valueToken.Kind}.");
                }
                try
                {
                    var key = keyToken.Span.ToStringValue();
                    dic[key] = new EnvEntry(
                        Key: key,
                        RawValue: valueToken.ToStringValue(),
                        ValueStartIdx: valueToken.Position.Absolute,
                        ValueLength: valueToken.Span.Length);
                    i++; // Skip the value token since we already processed it
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to parse key-value pair for key '{keyToken.ToStringValue()}'.", ex);
                }
            }
        }
        return dic;
    }
}
