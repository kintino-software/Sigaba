using Sigaba.Cli.Services.ConsoleServices;
using Spectre.Console;

namespace Sigaba.Cli.Adaptors;

public static class ConsoleService
{
    extension(IAnsiConsole console)
    {
        public void WriteAppLogo()
        {
            console.Write(new FigletText("Sigaba"));
        }

        public void WriteErrorLine(string message)
        {
            console.MarkupLine(message.AsError());
        }

        public void WriteInfoLine(string message)
        {
            console.MarkupLine(message.AsInfo());
        }

        public void WriteWarningLine(string message)
        {
            console.MarkupLine(message.AsWarning());
        }

        public void WriteDefaultLine(string message)
        {
            console.MarkupLine(message);
        }

        public void WriteSuccessLine(string message)
        {
            console.MarkupLine(message.AsSuccess());
        }

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

        public string PromptForInput(string promptMessage)
        {
            var dirPath = console.Prompt(new TextPrompt<string>(promptMessage.AsInfo()));
            while (true)
            {

                if (string.IsNullOrWhiteSpace(dirPath))
                {
                    console.WriteErrorLine("Directory path cannot be empty. Please try again.");
                    continue;
                }

                if (!Directory.Exists(dirPath))
                {
                    console.WriteErrorLine($"The directory '{dirPath}' does not exist. Please try again.");
                    continue;
                }

                return dirPath;
            }
        }
    }
}
