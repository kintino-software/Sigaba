namespace Sigaba.App.Exceptions;

/// <summary>
/// Exception thrown when the project root directory cannot be found.
/// Project root is defined as the directory containing the tool settings file (e.g., "sigaba.json").
/// </summary>
internal class ProjectRootNotFoundException : Exception;
