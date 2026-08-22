using Sigaba.Primitives.FileSystem;
using Sigaba.Services;
using System.IO.Abstractions;

namespace Sigaba.App.Services.PrivateKeys;

internal partial class PrivateKeyPathResolver(IFileSystem fs, IEnvironmentVariables env)
{
    private FilePath GetDefaultPrivateKeyOutputPath(string projectId)
    {
        return fs.NewFilePath(Constants.SigabaSystemDir, projectId, Constants.PrivateKeyFileName);
    }
}

internal partial class PrivateKeyPathResolver : IPrivateKeyPathResolver
{
    FilePath IPrivateKeyPathResolver.GetDefaultSavePath(string projectId)
    {
        return GetDefaultPrivateKeyOutputPath(projectId);
    }

    IEnumerable<FilePath> IPrivateKeyPathResolver.GetPossibleLoadingPaths(DirPath projectRootPath, string projectId)
    {
        // by order of precedence:

        // #1. Get from environment variable
        var envVar = env.GetEnvironmentVariable(Constants.PrivateKeyDirEnvVarKey);
        if (envVar != null)
        {
            yield return fs.NewFilePath(envVar, Constants.PrivateKeyFileName);
        }

        // #2. Get from project directory
        yield return projectRootPath.CombineAsFile(Constants.PrivateKeyFileName);

        // #3. Get from default system directory
        yield return GetDefaultPrivateKeyOutputPath(projectId);
    }

}
