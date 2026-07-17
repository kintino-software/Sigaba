using Kintino.CipherConf.IO.Primitives;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.IO.Services;

internal interface IDataSerializer
{
    ToolSettings DeserializeToolSettings(string jsonString);
    string SerializeToolSettings(ToolSettings toolSettings);
    PublicKey DeserializePublicKey(string str);
    string SerializePublicKey(PublicKey publicKey);
    PrivateKey DeserializePrivateKey(string str);
    string SerializePrivateKey(PrivateKey privateKey);
}