using Kintino.CipherConf.Primitives;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Models;

public class CipherPackTest
{
    private static Nonce CreateNonce() => new(new PlainData([1, 2, 3]));
    private static EncryptedData CreateEncryptedData() => new([1, 2, 3]);

    //

    [Fact]
    public void Should_return_string_representation()
    {
        var nonce = CreateNonce();
        var data = CreateEncryptedData();
        var version = 22;
        var type = JsonValueKind.Number;
        var cipherPack = new CipherPack(data, nonce, type, version);

        var result = cipherPack.Pack();

        result.Should().MatchRegex(@"^[A-Za-z0-9+/=]+\.[A-Za-z0-9+/=]+\.[A-Za-z0-9+/=]+.\d");
    }

    [Fact]
    public void Should_split_all_parts()
    {
        var expectedNonce = CreateNonce();
        var expectedVersion = 22;
        var expectedData = CreateEncryptedData();
        var expectedType = JsonValueKind.String;
        var cipherPack = new CipherPack(expectedData, expectedNonce, expectedType, expectedVersion);
        var merged = cipherPack.Pack();

        var (actualData, actualNonce, actualType, actualVersion) = CipherPack.Unpack(merged);

        actualVersion.Should().Be(expectedVersion);
        actualType.Should().Be(expectedType);
        actualNonce.Should().BeEquivalentTo(expectedNonce);
        actualData.Should().BeEquivalentTo(expectedData);
    }
}

