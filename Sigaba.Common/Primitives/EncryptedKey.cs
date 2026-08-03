using Sigaba.Primitives.Base;

namespace Sigaba.Primitives;

public record EncryptedKey(byte[] Bytes) : EncryptedByteLike<EncryptedKey>(Bytes);

