using Sigaba.Primitives;
using System.Text;

namespace Sigaba.Crypto.Services.Ciphers.V1;

public class CipherV1Test
{
    private readonly CipherV1 service = new();

    [Fact]
    public void Should_create_public_and_private_keys()
    {
        var (publicKey, privateKey) = service.GenerateKeys();

        publicKey.Bytes.Should().NotBeEmpty();
        privateKey.Bytes.Should().NotBeEmpty();
    }

    [Fact]
    public void Should_encrypt_plain_data()
    {
        var (publicKey, privateKey) = service.GenerateKeys();
        var original = new PlainData(Encoding.UTF8.GetBytes("This is a secrete message."));

        var encryptedData = service.Encrypt(original, publicKey);

        encryptedData.Should().NotBeEquivalentTo(original);
    }

    [Fact]
    public void Should_decrypt_encrypted_data()
    {
        var (publicKey, privateKey) = service.GenerateKeys();
        var original = new PlainData(Encoding.UTF8.GetBytes("This is a secrete message."));

        var encryptedData = service.Encrypt(original, publicKey);
        var decryptedData = service.Decrypt(encryptedData, privateKey);

        decryptedData.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void Should_throw_exception_decrypting_with_invalid_private_key()
    {
        var (publicKey, privateKey) = service.GenerateKeys();
        var original = new PlainData(Encoding.UTF8.GetBytes("This is a secrete message."));

        var encryptedData = service.Encrypt(original, publicKey);
        var action = () => service.Decrypt(encryptedData, new PrivateKey(new byte[32]));

        action.Should().Throw<Exception>();
    }
}

