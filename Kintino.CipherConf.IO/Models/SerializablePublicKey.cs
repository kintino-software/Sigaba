using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.IO.Models;

internal record SerializablePublicKey(PublicKey PublicKey) : IJsonSerializable<SerializablePublicKey>
{
    public static SerializablePublicKey DeserializeFromJsonString(string str)
    {
        return new SerializablePublicKey(new PublicKey(new PlainData(str.FromBase64String())));
    }
    public string SerializeToJsonString()
    {
        return this.PublicKey.Bytes.ToBase64String();
    }
}