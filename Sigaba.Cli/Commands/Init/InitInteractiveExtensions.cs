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
                var password = console.Prompt(new TextPrompt<string>(promptMessage.AsInfo()).Secret());

                if (string.IsNullOrWhiteSpace(password))
                {
                    console.WriteErrorLine("Password cannot be empty. Please try again.");
                    continue;
                }

                var confirmPassword = console.Prompt(
                    new TextPrompt<string>("Confirm the private key password:".AsInfo()).Secret());

                if (password == confirmPassword)
                {
                    return password;
                }
                else
                {
                    console.WriteErrorLine("Passwords do not match. Please try again.");
                }
            }
        }

    }
}
