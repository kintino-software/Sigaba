namespace Sigaba.Documents.Services.Env;

public record EnvEntry(string Key, string RawValue, int ValueStartIdx, int ValueLength);
