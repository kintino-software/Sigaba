using Kintino.CipherConf.IO;
using Kintino.CipherConf.Models;

namespace Kintino.CipherConfig;

public class FakeFileOperations : IFileOperations
{
    public ValueTask CopyWithOverwrite(string originalFilePath, string newFilePath)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask<IEnumerable<string>> GetFilesFromDirectory(string directory, IFileFilter fileFilter)
    {
        return new ValueTask<IEnumerable<string>>(Array.Empty<string>());
    }

    public ValueTask WithTempFile(string originalFile, TempFileEditOperation editingOperation, TempFileBeforeDeleteOperation beforeDeleteOperation)
    {
        return ValueTask.CompletedTask;
    }
}
