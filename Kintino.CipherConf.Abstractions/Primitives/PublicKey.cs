namespace Kintino.CipherConf.Primitives;

public record PublicKey(byte[] Bytes) : PlainByteLike<PublicKey>(Bytes);