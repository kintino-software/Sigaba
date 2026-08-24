namespace Sigaba.Cli.Models;

internal interface IGlobalOptions
{
    VerbosityLevel Verbosity { get; }
    void SetVerbosity(VerbosityLevel verbosityLevel);
}