using Sigaba.Primitives.Base;

namespace Sigaba.Primitives;

public record PlainData(byte[] Bytes) : PlainByteLike<PlainData>(Bytes);