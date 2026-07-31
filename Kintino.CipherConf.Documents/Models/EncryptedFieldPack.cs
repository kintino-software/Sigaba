using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Documents.Models;

internal record EncryptedFieldPack(
    EncryptedKey EncryptedKey,
    EncryptedData EncryptedData,
    Nonce Nonce);
