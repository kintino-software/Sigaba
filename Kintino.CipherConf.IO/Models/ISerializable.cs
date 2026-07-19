namespace Kintino.CipherConf.IO.Models;

internal interface ISerializable;

internal interface IJsonSerializable<T> : ISerializable where T : ISerializable
{
    string SerializeToJsonString();
    static abstract T DeserializeFromJsonString(string str);
}

internal interface IPlainTextSerializable<T> : ISerializable where T : ISerializable
{
    string SerializeToPlainTextString();
    static abstract T DeserializeFromPlainTextString(string str);
}


