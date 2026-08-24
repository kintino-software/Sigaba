namespace Sigaba.Cli.Models;

internal class GlobalOptions : IGlobalOptions
{
    private readonly Lock lockObj = new();

    private VerbosityLevel verbosity = VerbosityLevel.Normal;
    VerbosityLevel IGlobalOptions.Verbosity => verbosity;

    void IGlobalOptions.SetVerbosity(VerbosityLevel verbosityLevel)
    {
        lock (lockObj)
        {
            verbosity = verbosityLevel;
        }
    }
}
