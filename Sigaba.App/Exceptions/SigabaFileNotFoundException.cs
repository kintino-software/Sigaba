namespace Sigaba.App.Exceptions;

internal class SigabaFileNotFoundException(string location)
    : FileNotFoundException($"Sigaba file not found at '{location}'.");