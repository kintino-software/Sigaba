namespace Kintino.CipherConf.Primitives;

public record Nonce(byte[] Bytes) : PlainByteLike<Nonce>(Bytes);
