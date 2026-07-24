using Kintino.CipherConf.App.Dependencies;

namespace Kintino.CipherConf.App.Services;

public interface IEncryptConfigApp
{
    ValueTask Init(string targetFolder);
    ValueTask CipherFiles(string targetFolder);
    ValueTask DecipherFiles(string targetFolder);
    ValueTask EditFile(ITextEditor textEditor, string targetFolder, string editingFilePath);
}
