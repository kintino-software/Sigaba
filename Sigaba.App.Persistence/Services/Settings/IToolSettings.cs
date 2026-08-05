using System.IO.Abstractions;

namespace Sigaba.App.Services.Settings;

/// <summary>
/// Base interface for all settings classes.
/// </summary>
internal interface IToolSettings
{
    /// <summary>Gets the version of the settings.</summary>
    int Version { get; }
    /// <summary>
    /// Gets a predicate function that determines whether a given field name should be included in the working set of files.
    /// </summary>
    /// <param name="fieldName">The name of the field to check.</param>
    /// <returns>True if the field should be included; otherwise, false.</returns>
    bool FieldNamePredicate(string fieldName);
    /// <summary>
    /// Gets the working set of files starting from the specified folder, using the provided file system abstraction.
    /// </summary>
    /// <param name="fs">The file system abstraction to use.</param>
    /// <param name="startFolder">The folder from which to start the search.</param>
    /// <returns>An enumerable of file paths in the working set.</returns>
    IEnumerable<string> GetFilesWorkingSet(IFileSystem fs, string startFolder);
    /// <summary>
    /// Serializes the settings to a string.
    /// </summary>
    /// <returns>The serialized settings.</returns>
    string Serialize();
}

/// <summary>
/// Base interface for settings classes that receives a generic type parameter for the implementing class. 
/// This allows for static methods to be defined in the interface that can be called on the implementing class.
/// </summary>
/// <typeparam name="TSelf">The type of the implementing class.</typeparam>
internal interface IToolSettings<TSelf> : IToolSettings where TSelf : IToolSettings
{
    /// <summary>
    /// Creates a default instance of the implementing settings class.
    /// Usually, this method is used to provide a default configuration for the tool settings.
    /// </summary>
    /// <returns>The default instance of the implementing settings class.</returns>
    static abstract TSelf CreateDefault();
    /// <summary>
    /// Deserializes the specified string into an instance of the implementing settings class.
    /// </summary>
    /// <param name="serialized">The serialized string representation of the settings.</param>
    /// <returns>The deserialized instance of the implementing settings class.</returns>
    static abstract TSelf Deserialize(string serialized);
}
