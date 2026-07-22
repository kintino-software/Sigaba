using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents.Models;
using Kintino.CipherConf.Documents.TestHelpers;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Documents.Services;

public class DocumentCipherTest : BaseTest
{
    private readonly INonceGenerator nonceGenerator = Substitute.For<INonceGenerator>();
    private readonly PlainKey key = new(new([1, 2, 3]));

    private DocumentCipher CreateService()
    {
        nonceGenerator.NewNonce().Returns(new Nonce(new([4, 5, 6])));
        return new DocumentCipher(this.SymmetricCipher, nonceGenerator);
    }

    // Encrypt

    [Fact]
    public void Should_encrypt_json_and_back()
    {
        var original = """
        {
            "secret_text": "text",
            "secret_number": 123,
            "secret_boolean": true,
            "parent": {
                "secret_null": null,
                "secret_array": [1, 2, 3],
                "secret_object": {
                    "nested_secret": "nested value"
                }
            }
        }
        """;
        var service = CreateService();

        var encrypted = service.Encrypt(DocumentType.Json, original, key, field => field.Contains("secret_"));
        var decrypted = service.Decrypt(DocumentType.Json, encrypted, key);

        encrypted.Should().NotBe(original);
        decrypted.Should().Be(original);
    }

    [Fact]
    public void Encrypt_should_encrypt_filtered_nodes()
    {

    }

    // Decrypt

    [Fact]
    public void Decrypt_should_decrypt_document()
    {

    }
}

