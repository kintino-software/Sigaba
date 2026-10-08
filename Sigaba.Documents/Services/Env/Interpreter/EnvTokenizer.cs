using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Sigaba.Documents.Services.Env.Interpreter;

internal static class EnvGrammar
{
    public static TextParser<TextSpan> Comment { get; } = Span.Regex(@"#[^\r\n]*");
    public static TextParser<TextSpan> Key { get; } = Span.Regex(@"[A-Za-z_][A-Za-z0-9_.-]*(?=\s*=)");
    public static TextParser<TextSpan> Value { get; } = Span.Regex(@"[^#\r\n]+");
    public static TextParser<TextSpan> LiteralValue { get; } = Span.Regex(@"""(?:\\.|[^""])*""");
}


internal static class EnvTokenizer
{
    private static readonly Tokenizer<TokenType> Tokenizer =
        new TokenizerBuilder<TokenType>()
            .Ignore(Character.WhiteSpace)
            .Ignore(Character.EqualTo('='))
            .Match(EnvGrammar.Comment, TokenType.Comment)
            .Match(EnvGrammar.Key, TokenType.Key)
            .Match(EnvGrammar.LiteralValue, TokenType.LiteralValue)
            .Match(EnvGrammar.Value, TokenType.Value)
            .Build();

    public static TokenList<TokenType> Tokenize(string input)
    {
        var tokens = Tokenizer.Tokenize(input);
        return tokens;
    }
}
