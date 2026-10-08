using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace Sigaba.Documents.Services.Env.Interpreter;

internal static class EnvGrammar
{
    public static TextParser<TextSpan> Comment { get; } = Span.Regex(@"#[^\r\n]*(?:\r?\n|$)");
    public static TextParser<TextSpan> Key { get; } = Span.Regex(@"[A-Za-z_][A-Za-z0-9_.-]*(?=\s*=)");
    public static TextParser<TextSpan> LiteralValue { get; } = Span.Regex(@"\""(?:\\.|[^""])*\""(?:\r?\n|$)");
    public static TextParser<TextSpan> LiteralEscapedValue { get; } = Span.Regex(@"\'(?:\\.|[^'])*\'(?:\r?\n|$)");
    public static TextParser<TextSpan> MultiLineValue { get; } = Span.Regex(@"[^#\r\n\\]+(?:\\\r?\n\s*[^#\r\n\\]+)+(?:\r?\n|$)");
    public static TextParser<TextSpan> Value { get; } = Span.Regex(@"[^#\r\n\\]+(?:\r?\n|(?=\s*#)|$)");
}
