using Kintino.CipherConf.App.Primitives;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Models;

internal record CipherPack(CryptoBytes CipherBytes, Nonce Nonce, JsonValueKind ValueKind, int Version)
{
    public const char Separator = '.';

    public static CipherPack Unpack(string package)
    {
        var parts = package.Split(Separator);
        if (parts.Length != 4)
        {
            throw new InvalidOperationException("Invalid package format");
        }

        var nonce = new Nonce(new String64(parts[0]).AsBytes());
        var cipherBytes = new CryptoBytes(new String64(parts[1]).AsBytes());
        var valueKind = Enum.Parse<JsonValueKind>(parts[2]);
        var version = int.Parse(parts[3]);

        return new CipherPack(cipherBytes, nonce, valueKind, version);
    }

    public string Pack()
    {
        // convert all values explictly to avoid any implicit conversion issues
        var nonceBase64 = Nonce.Bytes.AsBase64();
        var cipherBytesBase64 = CipherBytes.Bytes.AsBase64();
        var valueKindString = ValueKind.ToString();
        var versionString = Version.ToString();
        return $"{nonceBase64}{Separator}{cipherBytesBase64}{Separator}{valueKindString}{Separator}{versionString}";
    }
}
