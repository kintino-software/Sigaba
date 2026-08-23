namespace Sigaba.Cli.Models;

internal class GlobalOptions : IGlobalOptions
{
    public VerbosityLevel Verbosity { get; set; } = VerbosityLevel.Normal;

    public static GlobalOptions ParseFromArgs(string[] args, out string[] remainingArgs)
    {
        var options = new GlobalOptions();
        var remainingArgsList = new List<string>();
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-v":
                case "--verbosity":
                    if (i + 1 < args.Length && Enum.TryParse<VerbosityLevel>(args[i + 1], true, out var logLevel))
                    {
                        options.Verbosity = logLevel;
                        i++; // Skip the next argument since it's the value for --verbosity
                    }
                    break;
                case "-q":
                case "--quiet":
                    options.Verbosity = VerbosityLevel.Quiet;
                    break;
                default:
                    remainingArgsList.Add(args[i]);
                    break;
            }
        }
        remainingArgs = [.. remainingArgsList];
        return options;
    }

}
