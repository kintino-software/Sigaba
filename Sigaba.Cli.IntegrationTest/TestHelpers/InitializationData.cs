namespace Sigaba.Cli.TestHelpers;

/// <summary>
/// Represents information regarding on how and where the app was initialized.
/// </summary>
/// <param name="Password">The password used to initialize the app.</param>
/// <param name="Cwd">The current working directory where the app was initialized.</param>
public record InitializationData(string Password, string Cwd);
