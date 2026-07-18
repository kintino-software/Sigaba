namespace Kintino.CipherConf.IO.Models;

internal interface ISerializable;

internal interface ISerializable<T> : ISerializable where T : ISerializable
{
    string Serialize();
    static abstract T Deserialize(string str);
}


