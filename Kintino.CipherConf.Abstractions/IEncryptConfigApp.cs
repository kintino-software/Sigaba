namespace Kintino.CipherConf;

public interface IEncryptConfigApp
{
    ValueTask Init(string targetFolder);
    ValueTask CipherFiles(string targetFolder);
    ValueTask DecipherFiles(string targetFolder);
    ValueTask EditFile(string targetFolder, string editingFilePath);
}
