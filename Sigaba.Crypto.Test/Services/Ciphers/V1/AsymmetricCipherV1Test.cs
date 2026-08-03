using Sigaba.Primitives;
using System.Security.Cryptography;

namespace Sigaba.Crypto.Services.Ciphers.V1;

public class AsymmetricCipherV1Test : BaseTest
{
    private readonly IAsymmetricCipher service = new AsymmetricCipherV1();

    // Create

    [Fact]
    public void Should_create_public_and_private_keys()
    {
        var (publicKey, privateKey) = service.CreateNewKeyPair();

        publicKey.Bytes.Length.Should().BePositive();
        privateKey.Bytes.Length.Should().BePositive();
    }

    [Fact]
    public void Should_create_unique_keys()
    {
        var (publicKey1, privateKey1) = service.CreateNewKeyPair();
        var (publicKey2, privateKey2) = service.CreateNewKeyPair();

        publicKey1.Should().NotBeEquivalentTo(publicKey2);
        privateKey1.Should().NotBeEquivalentTo(privateKey2);
    }

    // Encrypt

    [Fact]
    public void Should_encrypt_data()
    {
        var original = new PlainKey(GetRandomBytes());
        var (publicKey, _) = service.CreateNewKeyPair();

        var result = service.Encrypt(original, publicKey);

        result.Should().NotBeEquivalentTo(original);
        result.Bytes.Should().HaveCountGreaterThan(0);
    }

    // Decrypt

    [Fact]
    public void Should_decrypt_data()
    {
        var original = new PlainKey(GetRandomBytes());
        var (publicKey, privateKey) = service.CreateNewKeyPair();
        var encrypted = service.Encrypt(original, publicKey);

        var result = service.Decrypt(encrypted, privateKey);

        result.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void Should_throw_exception_when_decrypting_with_invalid_key()
    {
        var original = new PlainKey(GetRandomBytes());
        var (publicKey, privateKey) = service.CreateNewKeyPair();
        var encrypted = service.Encrypt(original, publicKey);
        var (_, invalidPrivateKey) = service.CreateNewKeyPair();

        Action act = () => service.Decrypt(encrypted, invalidPrivateKey);

        act.Should().Throw<CryptographicException>();
    }
}

