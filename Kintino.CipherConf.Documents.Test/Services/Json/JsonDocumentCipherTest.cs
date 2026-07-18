using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents.TestHelpers;
using Kintino.CipherConf.Primitives;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Kintino.CipherConf.Documents.Services.Json;

public class JsonDocumentCipherTest : BaseTest
{
    private readonly IValueCipher valueCipher;
    private readonly INonceGenerator nonceGenerator = Substitute.For<INonceGenerator>();
    private readonly PlainKey key = new(new([1, 2, 3]));
    private readonly Nonce nonce = new(new([4, 5, 6]));

    public JsonDocumentCipherTest()
    {
        nonceGenerator.NewNonce().Returns(nonce);

        // we use the concrete ValueCipher implementation for testing, because:
        // 1: mocking it would be cumbersome due to the number of edge cases and behaviors we would need to replicate
        // 2: it must work really tightly with json document cipher and we want to check its behaviour with complex json documents
        valueCipher = new ValueCipher(SymmetricCipher, nonceGenerator);
    }

    private IDocumentCipher CreateService()
    {
        return new JsonDocumentCipher(valueCipher);
    }

    private static void AssertDocumentsAreEquivalent(string actual, string expected)
    {
        // reserialize both documents to ensure that they are equivalent, ignoring whitespace and formatting differences
        JsonSerializerOptions options = new() { WriteIndented = true };
        var expectedSanitized = JsonNode.Parse(expected).ToJsonString(options);
        var actualSanitized = JsonNode.Parse(actual).ToJsonString(options);
        actualSanitized.Should().Be(expectedSanitized);
    }

    // Encrypt

    [Fact]
    public void Should_encrypt_all_properties_when_no_filter_is_provided()
    {
        var json = """
        {
          "name": "John Doe",
          "email": "john.doe@example.com"
        }
        """;
        var service = CreateService();

        //

        var result = service.Encrypt(key, json, null);

        //

        var rootNode = JsonNode.Parse(result);
        rootNode["name"].GetValue<string>().Should().NotBe("John Doe");
        rootNode["email"].GetValue<string>().Should().NotBe("john.doe@example.com");
    }

    [Fact]
    public void Should_encrypt_filtered_properties_when_filter_is_provided()
    {
        var plain = """
        {
          "name": "John Doe",
          "email": "john.doe@example.com"
        }
        """;
        var service = CreateService();

        //

        var result = service.Encrypt(key, plain, p => p == "email");

        //
        var rootNode = JsonNode.Parse(result);
        rootNode["name"].GetValue<string>().Should().Be("John Doe");
        rootNode["email"].GetValue<string>().Should().NotBe("john.doe@example.com");
    }

    // Decrypt

    [Fact]
    public void Should_decrypt_encrypted_properties()
    {
        var original = """
        {
          "name": "John Doe",
          "email": "john.doe@example.com"
        }
        """;
        var service = CreateService();
        var encrypted = service.Encrypt(key, original, null);

        //

        var result = service.Decrypt(key, encrypted);

        //

        var rootNode = JsonNode.Parse(result);
        rootNode["name"].GetValue<string>().Should().Be("John Doe");
        rootNode["email"].GetValue<string>().Should().Be("john.doe@example.com");
    }

    // Round-trip test

    [Fact]
    public void Should_handle_complex_json()
    {
        var original = """
        {
            "app": {
            "name": "MyApp",
            "version": "3.1.4",
            "debug": true,
            "maxRetries": 5
            },
            "database": {
            "host": "db.internal",
            "port": 5432,
            "credentials": {
                "username": "admin",
                "password": "s3cr3t!"
            },
            "replicaHosts": ["replica1.internal", "replica2.internal"]
            },
            "features": [
            { "name": "darkMode", "enabled": true },
            { "name": "betaApi",  "enabled": false }
            ],
            "metadata": {
            "createdAt": "2026-01-15T08:30:00Z",
            "tags": ["production", "us-east", "tier-1"],
            "notes": null
            }
        }
        """;
        var service = CreateService();

        //

        var cipher = service.Encrypt(key, original, p => p == "password" || p == "tags");
        var plain = service.Decrypt(key, cipher);

        //

        var cipherRoot = JsonNode.Parse(cipher);
        cipherRoot["app"]["name"].GetValue<string>().Should().Be("MyApp");
        cipherRoot["app"]["version"].GetValue<string>().Should().Be("3.1.4");
        cipherRoot["app"]["debug"].GetValue<bool>().Should().BeTrue();
        cipherRoot["app"]["maxRetries"].GetValue<int>().Should().Be(5);
        cipherRoot["database"]["host"].GetValue<string>().Should().Be("db.internal");
        cipherRoot["database"]["port"].GetValue<int>().Should().Be(5432);
        cipherRoot["database"]["credentials"]["username"].GetValue<string>().Should().Be("admin");
        cipherRoot["database"]["credentials"]["password"].GetValue<string>().Should().NotBe("s3cr3t!"); // encrypted
        cipherRoot["database"]["replicaHosts"][0].GetValue<string>().Should().Be("replica1.internal");
        cipherRoot["database"]["replicaHosts"][1].GetValue<string>().Should().Be("replica2.internal");
        cipherRoot["features"][0]["name"].GetValue<string>().Should().Be("darkMode");
        cipherRoot["features"][0]["enabled"].GetValue<bool>().Should().BeTrue();
        cipherRoot["features"][1]["name"].GetValue<string>().Should().Be("betaApi");
        cipherRoot["features"][1]["enabled"].GetValue<bool>().Should().BeFalse();
        cipherRoot["metadata"]["createdAt"].GetValue<string>().Should().Be("2026-01-15T08:30:00Z");
        cipherRoot["metadata"]["tags"].GetValueKind().Should().Be(JsonValueKind.String); // encrypted
        AssertDocumentsAreEquivalent(plain, original);

    }
}

