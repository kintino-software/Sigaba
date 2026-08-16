using Spectre.Console;

namespace Sigaba.Cli.Adaptors;

public static class ConsoleService
{
    extension(IAnsiConsole console)
    {
        public void WriteAppLogo()
        {
            console.Write(new FigletText("Sigaba"));
            console.WriteLine();
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

    }
}
