namespace Kintino.CipherConf.Primitives;

public record EncryptedKey
{
    public byte[] Bytes { get; }
    public EncryptedKey(EncryptedData encryptedData)
    {
        Bytes = encryptedData.Bytes;
    }
}

