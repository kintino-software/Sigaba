using Sigaba.Primitives.Crypto.Base;

namespace Sigaba.Primitives;

public record PublicKey(byte[] Bytes) : ByteLike<PublicKey>(Bytes), IPlainData;