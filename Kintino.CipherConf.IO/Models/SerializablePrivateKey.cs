using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.IO.Models;

internal record SerializablePrivateKey(PrivateKey PrivateKey) : ISerializable<SerializablePrivateKey>
{

    public static SerializablePrivateKey Deserialize(string str)
    {
        return new SerializablePrivateKey(new PrivateKey(new PlainData(str.FromBase64String())));
    }

    public string Serialize()
    {
        return this.PrivateKey.Bytes.ToBase64String();
    }
}
