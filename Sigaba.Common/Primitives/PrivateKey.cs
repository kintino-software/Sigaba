using Sigaba.Primitives.Base;

namespace Sigaba.Primitives;

public record PrivateKey(byte[] Bytes) : PlainByteLike<PrivateKey>(Bytes);