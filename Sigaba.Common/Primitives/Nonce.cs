using Sigaba.Primitives.Base;

namespace Sigaba.Primitives;

public record Nonce(byte[] Bytes) : PlainByteLike<Nonce>(Bytes);
