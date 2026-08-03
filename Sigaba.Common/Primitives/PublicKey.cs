using Sigaba.Primitives.Base;

namespace Sigaba.Primitives;

public record PublicKey(byte[] Bytes) : PlainByteLike<PublicKey>(Bytes);