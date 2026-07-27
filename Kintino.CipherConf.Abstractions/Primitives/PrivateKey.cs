namespace Kintino.CipherConf.Primitives;

public record PrivateKey(byte[] Bytes) : PlainByteLike<PrivateKey>(Bytes);