namespace Sigaba.App.Service;

public interface IFsHelper
{
    public Task WithTempFileAsync(string originalFile, Func<string, Task> editingOperation, Func<string, Task> beforeDeleteOperation);
    public Task CopyAndOverwrite(string sourceFilePath, string destinationFilePath);
}
