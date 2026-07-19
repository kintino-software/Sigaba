using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.IO.Models;

internal record SerializablePrivateKey(PrivateKey PrivateKey) : IJsonSerializable<SerializablePrivateKey>
{

    public static SerializablePrivateKey DeserializeFromJsonString(string str)
    {
        return new SerializablePrivateKey(new PrivateKey(new PlainData(str.FromBase64String())));
    }

    public string SerializeToJsonString()
    {
        return this.PrivateKey.Bytes.ToBase64String();
    }
}
