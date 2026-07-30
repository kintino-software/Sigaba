using Kintino.CipherConf.Primitives.Base;

namespace Kintino.CipherConf.Primitives;

public record PlainData(byte[] Bytes) : PlainByteLike<PlainData>(Bytes);