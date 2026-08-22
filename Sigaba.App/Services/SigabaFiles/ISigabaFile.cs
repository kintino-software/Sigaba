using Sigaba.Primitives.Crypto;
using Sigaba.Primitives.FileSystem;

namespace Sigaba.App.Services.SigabaFiles;

public interface ISigabaFile
{
    int Version { get; }
    string ProjectId { get; }
    PublicKey PublicKey { get; set; }
    bool FieldNamePredicate(string name);
    IEnumerable<FilePath> GetTargetFiles(DirPath rootFolder);

}



