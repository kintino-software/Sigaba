using Sigaba.Primitives.Base;

namespace Sigaba.Primitives;

public record EncryptedData(byte[] Bytes) : EncryptedByteLike<EncryptedData>(Bytes);
