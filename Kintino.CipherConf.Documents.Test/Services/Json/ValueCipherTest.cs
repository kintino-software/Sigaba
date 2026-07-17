using Kintino.CipherConf.Primitives;
using Kintino.CipherConfig;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Kintino.CipherConf.Documents.Services.Json;

public class ValueCipherTest
{
    private readonly PlainKey key = PlainKey.FakePlainKey();
    private readonly FakeSymmetricCipher symmetricCipher = new();
    private readonly FakeNonceGenerator nonceGenerator = new();

    private IValueCipher CreateService()
    {
        return new ValueCipher(symmetricCipher, nonceGenerator);
    }

    // CreateEncryptedValue

    [Fact]
    public void Should_encrypt()
    {
        var plainNode = JsonValue.Create("value");
        var service = CreateService();

        var actual = service.CreateEncryptedValue(plainNode, key);

        actual.GetValue<string>().Should().NotBe("value");
    }

    // CreateDecryptedValue

    [Theory]
    [InlineData("abcd")]
    [InlineData("")]
    [InlineData("📂📄⚙️❌➕")]
    [InlineData("abcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcd")]
    public void Should_decrypt_string(string expectedValue)
    {
        var original = JsonValue.Create(expectedValue);
        var service = CreateService();
        var encrypted = service.CreateEncryptedValue(original, key);

        var actual = service.CreateDecryptedValue(encrypted, key);

        actual.GetValueKind().Should().Be(JsonValueKind.String);
        actual.GetValue<string>().Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(22)]
    [InlineData(1000000)]
    [InlineData(0)]
    [InlineData(-100)]
    public void Should_decrypt_number(int expectedValue)
    {
        var original = JsonValue.Create(expectedValue);
        var service = CreateService();
        var encrypted = service.CreateEncryptedValue(original, key);

        var actual = service.CreateDecryptedValue(encrypted, key);

        actual.GetValueKind().Should().Be(JsonValueKind.Number);
        actual.GetValue<int>().Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_decrypt_boolean(bool expectedValue)
    {
        var original = JsonValue.Create(expectedValue);
        var service = CreateService();
        var encrypted = service.CreateEncryptedValue(original, key);

        var actual = service.CreateDecryptedValue(encrypted, key);

        actual.GetValueKind().Should().Be(expectedValue ? JsonValueKind.True : JsonValueKind.False);
        actual.GetValue<bool>().Should().Be(expectedValue);
    }

    [Fact]
    public void Should_decrypt_null()
    {
        var service = CreateService();
        var encrypted = service.CreateEncryptedValue(null, key); // JsonValue with null value is a null C# value

        var actual = service.CreateDecryptedValue(encrypted, key);

        actual.Should().BeNull();
    }

    [Fact]
    public void Should_decrypt_arrays()
    {
        var original = JsonNode.Parse("[1, 2, 3]");
        var service = CreateService();
        var encrypted = service.CreateEncryptedValue(original, key);

        var actual = service.CreateDecryptedValue(encrypted, key);

        actual.GetValueKind().Should().Be(JsonValueKind.Array);
        actual.AsArray().Select(n => n.GetValue<int>()).Should().BeEquivalentTo([1, 2, 3]);
    }

    [Fact]
    public void Should_decrypt_objects()
    {
        var original = JsonNode.Parse("{\"a\": 1, \"b\": 2, \"c\": 3}");
        var service = CreateService();
        var encrypted = service.CreateEncryptedValue(original, key);

        var actual = service.CreateDecryptedValue(encrypted, key);

        actual.GetValueKind().Should().Be(JsonValueKind.Object);
        actual.AsObject().ToDictionary(kv => kv.Key, kv => kv.Value.GetValue<int>()).Should().BeEquivalentTo(new Dictionary<string, int>
        {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 }
        });
    }

}

