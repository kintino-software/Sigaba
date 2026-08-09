using Sigaba.Primitives;
using System.IO.Abstractions;

namespace Sigaba.App.Services.SigabaFiles;

internal interface ISigabaFile
{
    int Version { get; }
    Guid ProjectId { get; }
    PublicKey PublicKey { get; set; }
    bool FieldNamePredicate(string name);
    IEnumerable<string> GetTargetFiles(IFileSystem fs, string rootFolder);

}



