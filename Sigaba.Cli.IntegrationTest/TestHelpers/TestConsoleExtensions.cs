using Spectre.Console.Testing;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Sigaba.Cli.IntegrationTest.TestHelpers;

internal static class TestConsoleExtensions
{
    public static void ShouldHaveOutputThatMatches(this TestConsole console, [StringSyntax("Regex")] string regexExpression)
    {
        var lines = regexExpression.Trim().Split('\n');
        var sanitizedRegexExp = string.Join("\n", lines.Select(line => line.TrimEnd()));

        var trimmedOutput = console.Output.Trim();
        trimmedOutput.Should().MatchRegex(new Regex(sanitizedRegexExp, RegexOptions.Multiline));
    }
}
