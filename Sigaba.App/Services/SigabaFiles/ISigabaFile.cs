using Sigaba.Primitives;

namespace Sigaba.App.Services.SigabaFiles;

public interface ISigabaFile
{
    int Version { get; }
    Guid ProjectId { get; }
    PublicKey PublicKey { get; set; }
    bool FieldNamePredicate(string name);
    IEnumerable<FilePath> GetTargetFiles(DirPath rootFolder);

}



