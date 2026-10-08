using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Sigaba.Documents.Services.Env.Interpreter;

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
            .Select(token => new Token<TokenType>(token.Kind, new TextSpan(token.ToStringValue().TrimEnd('\r', '\n'))))
            .ToArray();

        return new TokenList<TokenType>(tokens);
    }
}
