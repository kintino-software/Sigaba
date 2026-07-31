using Kintino.CipherConf.Documents.TestHelpers;
using System.IO.Abstractions.TestingHelpers;

namespace Kintino.CipherConf.Documents;

public class FileCipherTest : BaseTest
{
    private readonly MockFileSystem fs = new();
    private readonly FakeSymmetricCipher symmetricCipher = new();
    private readonly FakeAsymmetricCipher asymmetricCipher = new();
    private readonly Predicate<string> fieldFilter = (f) => f.Contains("_secret");

    private IFileCipher CreateService()
    {
        return new FileCipher(fs, symmetricCipher, asymmetricCipher);
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

        await service.CipherFile("test.json", asymmetricCipher.CorrectPublicKey, fieldFilter);

        var evaluator = JsonEvaluator.FromFile(fs.GetFile("test.json"));
        evaluator
            .AssertValueIs("$.b", 2)
            .AssertValueIs("$.c.e", "e value");
        evaluator
            .AssertValueIsNot("$.a_secret", "a value")
            .AssertValueIsNot("$.c.d_secret", "d value");
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

        await service.CipherFile("test.json", asymmetricCipher.CorrectPublicKey, fieldFilter);
        await service.DecipherFile("test.json", asymmetricCipher.CorrectPrivateKey);
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

        await service.CipherFile("test.json", asymmetricCipher.CorrectPublicKey, fieldFilter);
        await service.DecipherFile("test.json", asymmetricCipher.CorrectPrivateKey);
        var result = fs.GetFile("test.json").TextContents;

        result.Should().Be(originalJson);
    }

}

