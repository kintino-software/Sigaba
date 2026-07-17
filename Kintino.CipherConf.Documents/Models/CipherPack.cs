using Kintino.CipherConf.Primitives;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Models;

internal record CipherPack(EncryptedData EncryptedData, Nonce Nonce, JsonValueKind ValueKind, int Version)
{
    public const char Separator = '.';

    public string Pack()
    {
        var nonceStr = Nonce.Bytes.ToBase64String();                 // 0: nonce
        var encryptedDataStr = EncryptedData.Bytes.ToBase64String(); // 1: encrypted data
        var valueKindStr = ValueKind.ToString();                     // 2: value kind
        var versionStr = Version.ToString();                         // 3: version
        return $"{nonceStr}{Separator}{encryptedDataStr}{Separator}{valueKindStr}{Separator}{versionStr}";
    }

    public static CipherPack Unpack(string package)
    {
        var parts = package.Split(Separator);
        if (parts.Length != 4) // 4 components
        {
            throw new InvalidOperationException("Invalid package format");
        }

        var nonce = new Nonce(new PlainData(parts[0].FromBase64String())); // 0: nonce
        var encriptedData = new EncryptedData(parts[1].FromBase64String());  // 1: encrypted data
        var valueKind = Enum.Parse<JsonValueKind>(parts[2]);               // 2: value kind
        var version = int.Parse(parts[3]);                                 // 3: version

        return new CipherPack(encriptedData, nonce, valueKind, version);
    }


}
