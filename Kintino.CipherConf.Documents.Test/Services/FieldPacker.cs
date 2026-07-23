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
        var nonce = CreateNonce();
        var data = CreateEncryptedData();

        var result = FieldPacker.Pack(data, nonce);

        result.Should().MatchRegex(@"^ENC\([A-Za-z0-9+/=]+\)$");
    }

    // Unpack

    [Fact]
    public void Should_unpack_data_from_string()
    {
        var originalData = CreateEncryptedData();
        var originalNonce = CreateNonce();

        var pack = FieldPacker.Pack(originalData, originalNonce);

        var (actualData, actualNonce) = FieldPacker.Unpack(pack);

        actualData.Should().BeEquivalentTo(originalData);
        actualNonce.Should().BeEquivalentTo(originalNonce);
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

