using Kintino.CipherConf.Documents.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Documents.Services;

public class FieldPackerTest
{
    private static Nonce CreateNonce() => new(new PlainData([1, 2, 3]));
    private static EncryptedData CreateEncryptedData() => new([4, 5, 6]);

    // Pack

    [Fact]
    public void Should_pack_data_into_string()
    {
        var package = new EncryptedFieldPack(1, 2, 3, new EncryptedData([1, 2, 3]), new Nonce([4, 5, 6]));

        var result = FieldPacker.Pack(package);

        result.Should().MatchRegex(@"^ENC\([A-Za-z0-9+/=]+\)$");
    }

    // Unpack

    [Fact]
    public void Should_unpack_data_from_string()
    {
        var original = new EncryptedFieldPack(1, 2, 3, new EncryptedData([1, 2, 3]), new Nonce([4, 5, 6]));
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

