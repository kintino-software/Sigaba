using Microsoft.Extensions.Logging;
using Sigaba.Documents.TestHelpers;
using Sigaba.Primitives.Crypto;
using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.Documents;

public class FileCipherTest
{
    private readonly ILogger<FileCipher> logger = Substitute.For<ILogger<FileCipher>>();
    private readonly MockFileSystem fs = new();
    private readonly FakeCipher cipher = new FakeCipher().CheckKeysAndPasswords(false);
    private readonly Predicate<string> fieldFilter = (f) => f.Contains("_secret") || f.Contains("_SECRET");

    private IFileCipher CreateService()
    {
        return new FileCipher(fs, cipher, logger);
    }

    // CipherFile

    [Fact]
    public async Task Should_encrypt_json_documents()
    {
        var service = CreateService();
        var jsonDocument = """
        {
            "a_secret": "a value",
            "b": 2,
            "c": {
                "d_secret": "d value",
                "e": "e value"
            }
        }
        """;
        var filePath = fs.AddMockFilePath(jsonDocument, "test.json");

        await service.CipherFile(filePath, PublicKey.Any(), fieldFilter);

        var jsonTester = JsonTester.FromFile(fs, filePath);
        jsonTester.GetJsonValue<string>("$.a_secret").Should().NotBe("a value");
        jsonTester.GetJsonValue<int>("$.b").Should().Be(2);
        jsonTester.GetJsonValue<string>("$.c.d_secret").Should().NotBe("d value");
        jsonTester.GetJsonValue<string>("$.c.e").Should().Be("e value");
    }

    [Fact]
    public async Task Should_encrypt_env_documents()
    {
        var service = CreateService();
        var jsonDocument = """
        KEY1_SECRET=secret value 1
        KEY2=normal value 2
        KEY3_SECRET=secret value 3
        """;
        var filePath = fs.AddMockFilePath(jsonDocument, ".env");

        await service.CipherFile(filePath, PublicKey.Any(), fieldFilter);

        var newContent = fs.GetFile(filePath).TextContents;
        newContent.Should().NotContain("secret value 1");
        newContent.Should().Contain("normal value 2");
        newContent.Should().NotContain("secret value 3");
    }

    // DecipherFile

    [Fact]
    public async Task Should_decipher_json_documents()
    {
        var service = CreateService();
        var originalJson = """
        {
            "name_secret": "John Doe",
            "age": 30,
            "address": {
                "street_secret": "123 Main St",
                "city": "Anytown",
                "state": "CA",
                "zip": "12345"
            }
        }
        """;
        var filePath = fs.AddMockFilePath(originalJson, "test.json");

        await service.CipherFile(filePath, cipher.ThePublicKey, fieldFilter);
        await service.DecipherFile(filePath, cipher.ThePrivateKey);
        var actualJson = fs.GetFile(filePath).TextContents;

        actualJson.Should().Be(originalJson);
    }

    [Fact]
    public async Task Should_decipher_env_documents()
    {
        var service = CreateService();
        var originalJson = """
        KEY1_SECRET=secret value 1
        KEY2=normal value 2
        KEY3_SECRET=secret value 3
        """;
        var filePath = fs.AddMockFilePath(originalJson, ".env");

        await service.CipherFile(filePath, cipher.ThePublicKey, fieldFilter);
        await service.DecipherFile(filePath, cipher.ThePrivateKey);
        var actualJson = fs.GetFile(filePath).TextContents;

        actualJson.Should().Be(originalJson);
    }

    [Fact]
    public async Task Should_recover_original_format_when_deciphering_json_documents()
    {
        var service = CreateService();
        var originalJson = """
        {
            // comment
            "name_secret": [
                    "John",
                "Doe"
            ],
                    "age": 30,
            "address": {
                "street_secret": "123 Main St",
                                "city": "Anytown",
                    "state": "CA",
                "zip": "12345"
            },
        }
        """;
        var filePath = fs.AddMockFilePath(originalJson, "test.json");

        await service.CipherFile(filePath, PublicKey.Any(), fieldFilter);
        await service.DecipherFile(filePath, PrivateKey.Any());
        var result = fs.GetFile(filePath).TextContents;

        result.Should().Be(originalJson);
    }

    [Fact]
    public async Task Should_recover_original_format_when_deciphering_env_documents()
    {
        var service = CreateService();
        var originalJson = """
            KEY1_SECRET=secret value 1         
                              KEY2=normal value 2
                # comment
                    KEY3_SECRET=       secret value 3 # another comment
        """;
        var filePath = fs.AddMockFilePath(originalJson, ".env");

        await service.CipherFile(filePath, PublicKey.Any(), fieldFilter);
        await service.DecipherFile(filePath, PrivateKey.Any());
        var result = fs.GetFile(filePath).TextContents;

        result.Should().Be(originalJson);
    }
}

