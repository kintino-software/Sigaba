using System.Security.Cryptography;
using Kintino.CipherConf.Crypto.Services.Algos;

namespace Kintino.CipherConf.Crypto.Services.Algos;

public class RSACipherTest : BaseTest
{
    // Create

    [Fact]
    public void Should_create_public_and_private_keys()
    {
        RSACipher.CreateNewKeyPair(out var publicKey, out var privateKey);

        publicKey.Length.Should().BePositive();
        privateKey.Length.Should().BePositive();
    }

    [Fact]
    public void Should_create_unique_keys()
    {
        RSACipher.CreateNewKeyPair(out var publicKey1, out var privateKey1);
        RSACipher.CreateNewKeyPair(out var publicKey2, out var privateKey2);

        publicKey1.Should().NotBeEquivalentTo(publicKey2);
        privateKey1.Should().NotBeEquivalentTo(privateKey2);
    }

    // Encrypt

    [Fact]
    public void Should_encrypt_data()
    {
        byte[] original = this.GetRandomBytes();
        RSACipher.CreateNewKeyPair(out var publicKey, out _);

        var result = RSACipher.Encrypt(original, publicKey);

        result.Should().NotBeEquivalentTo(original);
        result.Should().HaveCountGreaterThan(0);
    }

    // Decrypt

    [Fact]
    public void Should_decrypt_data()
    {
        byte[] original = this.GetRandomBytes();
        RSACipher.CreateNewKeyPair(out var publicKey, out var privateKey);
        var encrypted = RSACipher.Encrypt(original, publicKey);

        var result = RSACipher.Decrypt(encrypted, privateKey);

        result.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void Should_throw_exception_when_decrypting_with_invalid_key()
    {
        byte[] original = this.GetRandomBytes();
        RSACipher.CreateNewKeyPair(out var publicKey, out var privateKey);
        var encrypted = RSACipher.Encrypt(original, publicKey);
        RSACipher.CreateNewKeyPair(out _, out var invalidPrivateKey);

        Action act = () => RSACipher.Decrypt(encrypted, invalidPrivateKey);

        act.Should().Throw<CryptographicException>();
    }
}

