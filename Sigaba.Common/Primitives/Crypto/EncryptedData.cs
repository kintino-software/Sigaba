using Sigaba.Primitives.Crypto.Base;

namespace Sigaba.Primitives.Crypto;

public record EncryptedData(byte[] Bytes) : ByteLike<EncryptedData>(Bytes), IEncryptedData;
