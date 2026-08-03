using System.IO.Abstractions;

namespace Kintino.CipherConf.App.Services.Settings;

internal interface IToolSettings
{
    int Version { get; }
    bool FieldNamePredicate(string fieldName);
    IEnumerable<string> GetFilesWorkingSet(IFileSystem fs, string startFolder);
}

internal interface IToolSettings<TSelf> : IToolSettings where TSelf : IToolSettings
{
    string Serialize();
    static abstract TSelf CreateDefault();
    static abstract TSelf Deserialize(string serialized);
}
