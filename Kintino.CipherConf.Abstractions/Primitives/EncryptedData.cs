namespace Kintino.CipherConf.Primitives;

public record EncryptedData(byte[] Bytes) : EncryptedByteLike<EncryptedData>(Bytes);
