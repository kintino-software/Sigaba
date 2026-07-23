namespace Kintino.CipherConf.Primitives;

public record EncryptedKey
{
    public byte[] Bytes { get; }
    public EncryptedKey(EncryptedData encryptedData)
    {
        Bytes = encryptedData.Bytes;
    }
    public static implicit operator EncryptedData(EncryptedKey encryptedKey) => new(encryptedKey.Bytes);
}

