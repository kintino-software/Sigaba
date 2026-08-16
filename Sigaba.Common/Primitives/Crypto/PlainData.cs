using Sigaba.Primitives.Crypto.Base;

namespace Sigaba.Primitives.Crypto;

public record PlainData(byte[] Bytes) : ByteLike<PlainData>(Bytes), IPlainData;