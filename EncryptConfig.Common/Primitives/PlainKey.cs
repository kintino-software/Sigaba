using Kintino.CipherConf.Primitives.Base;

namespace Kintino.CipherConf.Primitives;

public record PlainKey(byte[] Bytes) : PlainByteLike<PlainKey>(Bytes);

