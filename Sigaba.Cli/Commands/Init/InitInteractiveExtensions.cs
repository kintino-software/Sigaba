using Spectre.Console;

namespace Sigaba.Cli.Commands.Init;

internal static class InitInteractiveExtensions
{
    extension(IAnsiConsole console)
    {
        public string PromptForPasswordDefinition(string promptMessage)
        {
            while (true)
            {
                var password = console.Prompt(new TextPrompt<string>(promptMessage).Secret());

                if (string.IsNullOrWhiteSpace(password))
                {
                    console.MarkupLine("[red]Password cannot be empty. Please try again.[/]");
                    continue;
                }

                var confirmPassword = console.Prompt(
                    new TextPrompt<string>("Confirm the private key password:").Secret());

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

    }
}
