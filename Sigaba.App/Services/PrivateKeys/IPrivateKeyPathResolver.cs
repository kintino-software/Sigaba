using Sigaba.Primitives;
using Sigaba.Primitives.FileSystem;

namespace Sigaba.App.Services.PrivateKeys;

internal interface IPrivateKeyPathResolver
{
  FilePath GetDefaultSavePath(string projectId);
  IEnumerable<FilePath> GetPossibleLoadingPaths(DirPath projectRootPath, string projectId);
}
