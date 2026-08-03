namespace Sigaba.App;

/// <summary>
/// Abstraction for a text editor that can be used to edit files. 
/// This interface defines a method for editing a file given its file path. 
/// Implementations of this interface can provide different ways to open and edit files, such as using a command-line text editor or a graphical text editor.
/// </summary>
public interface ITextEditor
{
    /// <summary>
    /// Edits the specified file.
    /// </summary>
    /// <param name="filePath">The path of the file to edit.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task should be completed when the editor is closed, saving or not the file changes.</returns>
    Task EditFile(string filePath);
}
