namespace Sigaba.Documents.Services.Json.Parser;

internal record JsonToken(string Path, string RawValue, JsonDataType Type, int StartIndex, int Length);
