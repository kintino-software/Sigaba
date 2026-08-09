using Sigaba.Primitives.Base;

namespace Sigaba.Primitives;

public record PrivateKey(byte[] Bytes) : ByteLike<PrivateKey>(Bytes), IPlainData;