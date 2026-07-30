using Kintino.CipherConf.Primitives.Base;

namespace Kintino.CipherConf.Primitives;

public record PrivateKey(byte[] Bytes) : PlainByteLike<PrivateKey>(Bytes);