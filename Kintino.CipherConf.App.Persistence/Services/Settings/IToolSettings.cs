namespace Kintino.CipherConf.App.Services.Settings;

public interface IToolSettings
{
    Predicate<string> FieldFilter { get; }
    IEnumerable<string> WorkingSetFiles { get; }
}

