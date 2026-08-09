using Spectre.Console;
using System.IO.Abstractions;

namespace Sigaba.Cli.Interactive;

internal class InteractiveInit(IFileSystem fs, IAnsiConsole console)
{
    public record Result(string PrivateKeyPassword, string PrivateKeyFileLocation);

    public Result Run()
    {
        console.Write(new FigletText("Sigaba"));
        console.WriteLine();

        var privateKeyPassword = AskForPrivateKeyPassword();
        var privateKeyLocation = AsksForPrivateKeyFileLocation();

        return new Result(privateKeyPassword, privateKeyLocation);
    }

    private string AskForPrivateKeyPassword()
    {
        while (true)
        {
            var password = console.Prompt(
                    new TextPrompt<string>(@"[green]Enter the private key password[/]:")
                        .PromptStyle("red")
                        .Secret());

            if (string.IsNullOrWhiteSpace(password))
            {
                console.MarkupLine("[red]Password cannot be empty. Please try again.[/]");
                continue;
            }

            var confirmPassword = console.Prompt(
                new TextPrompt<string>(@"[green]Confirm the private key password[/]:")
                    .PromptStyle("red")
                    .Secret());

            if (password == confirmPassword)
            {
                return password;
            }
            else
            {
                console.MarkupLine("[red]Passwords do not match. Please try again.[/]");
            }
        }
    }

    private string AsksForPrivateKeyFileLocation()
    {
        var defaultLocation = fs.Directory.GetCurrentDirectory();
        while (true)
        {
            var location = console.Prompt(
                        new TextPrompt<string>(@"[green]Enter folder path where the generated private.key will be written[/]:")
                            .PromptStyle("red")
                            .DefaultValue(defaultLocation));

            if (fs.Directory.Exists(location))
            {
                return location;
            }
            else
            {
                console.MarkupLine($"[red]The directory '{location}' does not exist. Please try again.[/]");
            }
        }
    }
}
