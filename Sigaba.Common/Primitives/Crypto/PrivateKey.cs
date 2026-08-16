using Sigaba.Primitives.Crypto.Base;

namespace Sigaba.Primitives;

public record PrivateKey(byte[] Bytes) : ByteLike<PrivateKey>(Bytes), IPlainData;