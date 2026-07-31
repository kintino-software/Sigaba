using Kintino.CipherConf.Documents.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Documents.Services;

public class FieldPackerTest
{
    private readonly Nonce nonce = new(new PlainData([1, 2, 3]));
    private readonly EncryptedData encryptedData = new([4, 5, 6]);
    private readonly EncryptedKey encryptedKey = new([7, 8, 9]);

    // Pack

    [Fact]
    public void Should_pack_data_into_string()
    {
        var package = new EncryptedFieldPack(encryptedKey, encryptedData, nonce);

        var result = FieldPacker.Pack(package);

        result.Should().MatchRegex(@"^ENC\([A-Za-z0-9+/=]+\)$");
    }

    // Unpack

    [Fact]
    public void Should_unpack_data_from_string()
    {
        var original = new EncryptedFieldPack(encryptedKey, encryptedData, nonce);
        var pack = FieldPacker.Pack(original);

        var result = FieldPacker.Unpack(pack);

        result.Should().BeEquivalentTo(original);
    }

    // IsEncryptedFieldValue

    [Fact]
    public void Should_not_unpack_non_encrypted_value()
    {
        var nonEncryptedValue = "This is not an encrypted value";

        Action act = () => FieldPacker.Unpack(nonEncryptedValue);

        act.Should().Throw<ArgumentException>();
    }
}

