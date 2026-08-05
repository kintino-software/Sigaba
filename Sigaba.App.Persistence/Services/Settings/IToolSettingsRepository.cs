namespace Sigaba.App.Services.Settings;

/// <summary>
/// Defines a repository interface for loading and saving tool settings.
/// </summary>
internal interface IToolSettingsRepository
{
    /// <summary>
    /// Loads the tool settings from the specified file path.
    /// </summary>
    /// <param name="filePath">The path of the file from which to load the settings.</param>
    /// <returns>The loaded tool settings, or null if the file does not exist or cannot be read.</returns>
    Task<IToolSettings?> LoadAsync(string filePath);

    /// <summary>
    /// Saves the default tool settings to the specified file path.
    /// </summary>
    /// <param name="filePath">The path of the file to which to save the default settings.</param>
    Task SaveDefaultAsync(string filePath);
}
