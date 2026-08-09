using Sigaba.Primitives;
using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.Documents;

public class FileCipherTest
{
    private readonly MockFileSystem fs = new();
    private readonly FakeCipher cipher = new FakeCipher().CheckKeysAndPasswords(false);
    private readonly Predicate<string> fieldFilter = (f) => f.Contains("_secret");

    private IFileCipher CreateService()
    {
        return new FileCipher(fs, cipher);
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
        fs.AddFile("test.json", new MockFileData(jsonDocument));

        await service.CipherFile("test.json", PublicKey.Any(), fieldFilter);

        var jsonTester = JsonTester.FromFile(fs, "test.json");
        jsonTester.GetJsonValue<string>("$.a_secret").Should().NotBe("a value");
        jsonTester.GetJsonValue<int>("$.b").Should().Be(2);
        jsonTester.GetJsonValue<string>("$.c.d_secret").Should().NotBe("d value");
        jsonTester.GetJsonValue<string>("$.c.e").Should().Be("e value");
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
        fs.AddFile("test.json", new MockFileData(originalJson));

        await service.CipherFile("test.json", cipher.ThePublicKey, fieldFilter);
        await service.DecipherFile("test.json", cipher.ThePrivateKey);
        var actualJson = fs.GetFile("test.json").TextContents;

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
        fs.AddFile("test.json", new MockFileData(originalJson));

        await service.CipherFile("test.json", PublicKey.Any(), fieldFilter);
        await service.DecipherFile("test.json", PrivateKey.Any());
        var result = fs.GetFile("test.json").TextContents;

        result.Should().Be(originalJson);
    }

}

