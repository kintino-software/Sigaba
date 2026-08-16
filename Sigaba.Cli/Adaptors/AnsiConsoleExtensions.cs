namespace Sigaba.Cli.Services.ConsoleServices;

public static class AnsiConsoleExtensions
{
    extension(string str)
    {
        public string AsSuccess()
        {
            return $"[green]{str}[/]";
        }

        public string AsInfo()
        {
            return $"[blue]{str}[/]";
        }

        public string AsWarning()
        {
            return $"[yellow]{str}[/]";
        }

        public string AsError()
        {
            return $"[red]{str}[/]";
        }
    }
}