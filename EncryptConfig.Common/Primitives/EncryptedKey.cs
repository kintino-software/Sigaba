using Kintino.CipherConf.Primitives.Base;

namespace Kintino.CipherConf.Primitives;

public record EncryptedKey(byte[] Bytes) : EncryptedByteLike<EncryptedKey>(Bytes);

