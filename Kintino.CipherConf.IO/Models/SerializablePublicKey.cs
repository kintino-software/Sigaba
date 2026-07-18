using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.IO.Models;

internal record SerializablePublicKey(PublicKey PublicKey) : ISerializable<SerializablePublicKey>
{
    public static SerializablePublicKey Deserialize(string str)
    {
        return new SerializablePublicKey(new PublicKey(new PlainData(str.FromBase64String())));
    }
    public string Serialize()
    {
        return this.PublicKey.Bytes.ToBase64String();
    }
}