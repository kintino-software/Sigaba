namespace Kintino.CipherConf.App.Services;

public interface IECApp
{
    ValueTask Init(string targetFolder);
    ValueTask CipherFiles(string targetFolder);
    ValueTask DecipherFiles(string targetFolder);
    ValueTask EditFile(string targetFolder, string editingFilePath);
}
