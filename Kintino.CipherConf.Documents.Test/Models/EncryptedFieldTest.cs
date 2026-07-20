using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Documents.Models;

public class EncryptedFieldTest
{
    private static Nonce CreateNonce() => new(new PlainData([1, 2, 3]));
    private static EncryptedData CreateEncryptedData() => new([4, 5, 6]);

    //

    [Fact]
    public void Should_pack_data_into_string()
    {
        var nonce = CreateNonce();
        var data = CreateEncryptedData();
        var cipherPack = new CipherPack(data, nonce);

        var result = cipherPack.Pack();

        result.Should().MatchRegex(@"^ENC\([A-Za-z0-9+/=]+\)$");
    }

    [Fact]
    public void Should_unpack_data_from_string()
    {
        var expected = new CipherPack(CreateEncryptedData(), CreateNonce());
        var pack = expected.Pack();

        var success = CipherPack.TryUnpack(pack, out var actual);

        success.Should().BeTrue();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Should_not_unpack_non_encrypted_value()
    {
        var nonEncryptedValue = "This is not an encrypted value";

        var success = CipherPack.TryUnpack(nonEncryptedValue, out var actual);

        success.Should().BeFalse();
        actual.Should().BeNull();
    }
}

