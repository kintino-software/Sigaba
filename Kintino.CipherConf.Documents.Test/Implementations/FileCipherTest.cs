using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents.TestHelpers;
using Kintino.CipherConf.Models;
using System.IO.Abstractions.TestingHelpers;

namespace Kintino.CipherConf.Documents.Implementations;

public class FileCipherTest : BaseTest
{
    private readonly MockFileSystem fs = new();
    private readonly FakeSymmetricCipher symmetricCipherV1 = new();
    private readonly FakeAsymmetricCipher asymmetricCipherV1 = new();
    private readonly IFieldFilter fieldFilter = Substitute.For<IFieldFilter>();
    private readonly ICipherFactory cipherFactory = Substitute.For<ICipherFactory>();

    private IFileCipher CreateService()
    {
        cipherFactory.GetLatestAsymmetricCipher().ReturnsForAnyArgs(asymmetricCipherV1);
        cipherFactory.GetLatestSymmetricCipher().ReturnsForAnyArgs(symmetricCipherV1);
        cipherFactory.GetAsymmetricCipher(1).Returns(asymmetricCipherV1);
        cipherFactory.GetSymmetricCipher(1).Returns(symmetricCipherV1);
        fieldFilter.Match(default).ReturnsForAnyArgs(ci => ci.Arg<string>().Contains("_secret"));
        return new FileCipher(fs, cipherFactory);
    }

    // CipherFile

    [Fact]
    public async Task Should_encrypt_json_documents()
    {
        var service = CreateService();
        var jsonDocument = """
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
        fs.AddFile("test.json", new MockFileData(jsonDocument));

        await service.CipherFile("test.json", asymmetricCipherV1.CorrectPublicKey, fieldFilter);
        var result = fs.GetFile("test.json").TextContents;

        result.Should().NotBe(jsonDocument);
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

        await service.CipherFile("test.json", asymmetricCipherV1.CorrectPublicKey, fieldFilter);
        await service.DecipherFile("test.json", asymmetricCipherV1.CorrectPrivateKey);
        var result = fs.GetFile("test.json").TextContents;

        result.Should().Be(originalJson);
    }

    [Fact]
    public async Task Should_recover_original_format_when_deciphering_json_documents()
    {
        var service = CreateService();
        var originalJson = """
        {
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
            }
        }
        """;
        fs.AddFile("test.json", new MockFileData(originalJson));

        await service.CipherFile("test.json", asymmetricCipherV1.CorrectPublicKey, fieldFilter);
        await service.DecipherFile("test.json", asymmetricCipherV1.CorrectPrivateKey);
        var result = fs.GetFile("test.json").TextContents;

        result.Should().Be(originalJson);
    }

}

