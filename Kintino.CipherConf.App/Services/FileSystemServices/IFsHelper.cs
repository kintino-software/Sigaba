namespace Kintino.CipherConf.App.Services.FileSystemServices;

internal interface IFsHelper
{
    IEnumerable<string> Crawl(string rootDirFullPath, string[] includeGlob, string[] excludeGlob);
    public Task WithTempFileAsync(string originalFile, Func<string, Task> editingOperation, Func<string, Task> beforeDeleteOperation);
    public Task CopyAndOverwrite(string sourceFilePath, string destinationFilePath);
}
