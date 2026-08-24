namespace Sigaba.App.Exceptions;

internal class UnknownSigabaFileVersionException(int version)
    : NotSupportedException($"Sigaba file has version {version} which is an unknown version.");
