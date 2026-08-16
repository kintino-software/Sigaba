using Sigaba.Primitives.Crypto.Base;

namespace Sigaba.Primitives.Crypto;

public record PublicKey(byte[] Bytes) : ByteLike<PublicKey>(Bytes), IPlainData;