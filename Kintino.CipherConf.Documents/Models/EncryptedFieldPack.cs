using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Documents.Models;

internal record EncryptedFieldPack(
    int SymmetricCipherVersion,
    int AsymmetricCipherVersion,
    int KeyIndex,
    EncryptedData EncryptedData,
    Nonce Nonce);
