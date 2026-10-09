using Sigaba.Primitives.Crypto;
using Sigaba.Primitives.FileSystem;
using System.IO.Abstractions;

namespace Sigaba.App.Services.SigabaFiles;

public interface ISigabaFile
{
    int Version { get; }
    string ProjectId { get; }
    PublicKey PublicKey { get; set; }
    bool FieldNamePredicate(string name);
    IEnumerable<FilePath> GetTargetFiles(IFileSystem fs, DirPath rootFolder);
    bool IsTargetFile(IFileSystem fs, FilePath filePath, DirPath rootFolder);
}



