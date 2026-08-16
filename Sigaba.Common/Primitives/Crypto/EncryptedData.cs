using Sigaba.Primitives.Crypto.Base;

namespace Sigaba.Primitives;

public record EncryptedData(byte[] Bytes) : ByteLike<EncryptedData>(Bytes), IEncryptedData;
