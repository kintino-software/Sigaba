using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents.TestHelpers;
using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;
using System.IO.Abstractions.TestingHelpers;

namespace Kintino.CipherConf.Documents.Implementations;

public class FileCipherTest : BaseTest
{
    private readonly MockFileSystem fs = new();
    private readonly INonceGenerator nonceGenerator = Substitute.For<INonceGenerator>();
    private readonly IFieldFilter fieldFilter = Substitute.For<IFieldFilter>();
    private readonly Nonce nonce = new(new([1, 2, 3, 4]));
    private readonly PlainKey key = new(new([5, 6, 7, 8]));

    private IFileCipher CreateService()
    {
        fieldFilter.Match(default).ReturnsForAnyArgs(ci => ci.Arg<string>().Contains("_secret"));
        nonceGenerator.NewNonce().Returns(nonce);
        return new FileCipher(fs, nonceGenerator);
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

        await service.CipherFile("test.json", key, this.SymmetricCipher, fieldFilter);
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

        await service.CipherFile("test.json", key, this.SymmetricCipher, fieldFilter);
        await service.DecipherFile("test.json", key, this.SymmetricCipher);
        var result = fs.GetFile("test.json").TextContents;

        result.Should().Be(originalJson);
    }

}

