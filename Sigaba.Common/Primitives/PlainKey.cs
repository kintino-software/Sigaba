using Sigaba.Primitives.Base;

namespace Sigaba.Primitives;

public record PlainKey(byte[] Bytes) : PlainByteLike<PlainKey>(Bytes);

