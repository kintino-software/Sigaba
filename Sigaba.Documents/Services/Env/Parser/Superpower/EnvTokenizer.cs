using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Sigaba.Documents.Services.Env.Parser.Superpower;

internal static class EnvTokenizer
{
    private static readonly Tokenizer<TokenType> Tokenizer =
        new TokenizerBuilder<TokenType>()
            .Ignore(Character.In(' ', '\t'))
            .Ignore(Character.EqualTo('='))
            .Match(EnvGrammar.Comment, TokenType.Comment)
            .Match(EnvGrammar.Key, TokenType.Key)
            .Match(EnvGrammar.LiteralValue, TokenType.LiteralValue)
            .Match(EnvGrammar.MultiLineValue, TokenType.MultiLineValue)
            .Match(EnvGrammar.LiteralEscapedValue, TokenType.EscapedLiteralValue)
            .Match(EnvGrammar.Value, TokenType.Value)
            .Build();

    public static TokenList<TokenType> Tokenize(string input)
    {
        var tokens = Tokenizer.Tokenize(input)
            .Select(token =>
            {
                var value = token.Span.ToStringValue().TrimEnd('\r', '\n');
                var span = token.Span.Slice(0, value.Length);
                return new Token<TokenType>(token.Kind, span);
            })
            .ToArray();

        return new TokenList<TokenType>(tokens);
    }
}
