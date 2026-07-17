using Kintino.CipherConf.Primitives;
using Kintino.CipherConfig;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Models;

public class CipherPackTest
{
    [Fact]
    public void Should_return_string_representation()
    {
        var nonce = Nonce.FakeNonce();
        var data = EncryptedData.FakeEncryptedData();
        var version = 22;
        var type = JsonValueKind.Number;
        var cipherPack = new CipherPack(data, nonce, type, version);

        var result = cipherPack.Pack();

        result.Should().MatchRegex(@"^[A-Za-z0-9+/=]+\.[A-Za-z0-9+/=]+\.[A-Za-z0-9+/=]+.\d");
    }

    [Fact]
    public void Should_split_all_parts()
    {
        var expectedNonce = Nonce.FakeNonce();
        var expectedVersion = 22;
        var expectedData = EncryptedData.FakeEncryptedData();
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

