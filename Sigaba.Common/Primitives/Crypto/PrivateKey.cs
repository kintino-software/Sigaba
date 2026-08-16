using Sigaba.Primitives.Crypto.Base;

namespace Sigaba.Primitives.Crypto;

public record PrivateKey(byte[] Bytes) : ByteLike<PrivateKey>(Bytes), IPlainData;