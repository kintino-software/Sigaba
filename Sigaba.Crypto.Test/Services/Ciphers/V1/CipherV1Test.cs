using Sigaba.Primitives.Crypto;
using System.Text;

namespace Sigaba.Crypto.Services.Ciphers.V1;

public class CipherV1Test
{
    private readonly ICipher service = new CipherV1();

    // GenerateKeys

    [Fact]
    public void Should_create_public_and_private_keys()
    {
        var (publicKey, privateKey) = service.GenerateKeys();

        publicKey.Bytes.Should().NotBeEmpty();
        privateKey.Bytes.Should().NotBeEmpty();
    }

    // EncryptWithKey

    [Fact]
    public void Should_encrypt_plain_data()
    {
        var (publicKey, _) = service.GenerateKeys();
        var original = new PlainData(Encoding.UTF8.GetBytes("This is a secrete message."));

        var encryptedData = service.EncryptWithKey(original, publicKey);

        encryptedData.Should().NotBeEquivalentTo(original);
    }

    [Fact]
    public void Should_throw_when_encrypting_with_invalid_public_key()
    {
        var (publicKey, _) = service.GenerateKeys();
        var original = new PlainData(Encoding.UTF8.GetBytes("This is a secrete message."));
        publicKey = new PublicKey([.. publicKey.Bytes, 1]);

        var action = () => service.EncryptWithKey(original, publicKey);

        action.Should().Throw<Exception>();
    }

    // DecryptWithKey

    [Fact]
    public void Should_decrypt_encrypted_data()
    {
        var (publicKey, privateKey) = service.GenerateKeys();
        var original = new PlainData(Encoding.UTF8.GetBytes("This is a secrete message."));

        var encryptedData = service.EncryptWithKey(original, publicKey);
        var decryptedData = service.DecryptWithKey(encryptedData, privateKey);

        decryptedData.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void Should_throw_exception_decrypting_with_invalid_private_key()
    {
        var (publicKey, privateKey) = service.GenerateKeys();
        var original = new PlainData(Encoding.UTF8.GetBytes("This is a secrete message."));

        var encryptedData = service.EncryptWithKey(original, publicKey);
        var action = () => service.DecryptWithKey(encryptedData, new PrivateKey(new byte[32]));

        action.Should().Throw<Exception>();
    }

    // EncryptWithPassword

    [Fact]
    public void Should_encrypt_plain_data_with_password()
    {
        var password = "my-secret";
        var original = new PlainData(Encoding.UTF8.GetBytes("This is a secrete message."));

        var encryptedData = service.EncryptWithPassword(original, password);

        encryptedData.Should().NotBeEquivalentTo(original);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_throw_when_encrypting_data_with_null_or_empty_password(string password)
    {
        var original = new PlainData(Encoding.UTF8.GetBytes("This is a secrete message."));

        var action = () => service.EncryptWithPassword(original, password);

        action.Should().Throw<Exception>();
    }

    // DecryptWithPassword

    [Fact]
    public void Should_decrypt_encrypted_data_with_password()
    {
        var password = "my-secret";
        var original = new PlainData(Encoding.UTF8.GetBytes("This is a secrete message."));

        var encryptedData = service.EncryptWithPassword(original, password);
        var decryptedData = service.DecryptWithPassword(encryptedData, password);

        decryptedData.Should().BeEquivalentTo(original);
    }
}
