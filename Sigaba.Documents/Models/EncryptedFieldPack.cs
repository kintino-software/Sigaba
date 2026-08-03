using Sigaba.Primitives;

namespace Sigaba.Documents.Models;

internal record EncryptedFieldPack(
    EncryptedKey EncryptedKey,
    EncryptedData EncryptedData,
    Nonce Nonce);
