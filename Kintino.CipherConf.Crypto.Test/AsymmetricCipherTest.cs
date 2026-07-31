using Kintino.CipherConf.Crypto.Services.Ciphers;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Crypto;

public class AsymmetricCipherTest : BaseTest
{
    private readonly IVersionedAsymmetricCipher fakeCipherV1 = Substitute.For<IVersionedAsymmetricCipher>();
    private readonly IVersionedAsymmetricCipher fakeCipherV2 = Substitute.For<IVersionedAsymmetricCipher>();
    private readonly PublicKey publicKey = new([1, 2, 3, 16]);
    private readonly PrivateKey privateKey = new([5, 6, 7, 16]);
    private readonly EncryptedKey encryptedKey = new([9, 10, 11, 16]);
    private readonly PlainKey plainKey = new([13, 14, 15, 16]);

    public AsymmetricCipherTest()
    {
        fakeCipherV1.Version.Returns<byte>(1);
        fakeCipherV2.Version.Returns<byte>(2);
        foreach (var cipher in new IVersionedAsymmetricCipher[] { fakeCipherV1, fakeCipherV2 })
        {
            cipher.CreateNewKeyPair().Returns((publicKey, privateKey));
            cipher.Encrypt(default, default).ReturnsForAnyArgs(encryptedKey);
            cipher.Decrypt(default, default).ReturnsForAnyArgs(plainKey);
        }
    }

    private IAsymmetricCipher CreateService(params IVersionedAsymmetricCipher[] ciphers)
    {
        var service = new AsymmetricCipher(ciphers);
        return service;
    }

    private IAsymmetricCipher CreateService()
    {
        return CreateService(fakeCipherV1, fakeCipherV2);
    }

    // CreateNewPair

    [Fact]
    public void Should_create_key_pairs_with_correct_cipher_implementation()
    {
        var service = CreateService();

        var result = service.CreateNewKeyPair();

        fakeCipherV2.Received(1).CreateNewKeyPair();
    }

    // Encrypt

    [Fact]
    public void Should_encrypt_with_correct_cipher_implementation()
    {
        var service = CreateService();

        var result = service.Encrypt(plainKey, publicKey);

        fakeCipherV2.Received(1).Encrypt(plainKey, publicKey);
        result.Bytes.Should().HaveCount(encryptedKey.Bytes.Length + 1);
    }

    [Fact]
    public void Should_tag_encrypted_data()
    {
        var service = CreateService();

        var result = service.Encrypt(plainKey, publicKey);

        result.Bytes.Should().HaveCount(encryptedKey.Bytes.Length + 1);
        result.Bytes[0].Should().Be(2);
    }

    // Decrypt

    [Fact]
    public void Should_decrypt_with_correct_cipher_implementation()
    {
        var oldService = CreateService(fakeCipherV1);
        var oldEncryptedData = oldService.Encrypt(plainKey, publicKey); // if encrypts with older version
        var newService = CreateService(fakeCipherV1, fakeCipherV2);

        _ = newService.Decrypt(oldEncryptedData, privateKey);

        fakeCipherV1.Received(1).Encrypt(plainKey, publicKey);
        fakeCipherV1.Received(1).Decrypt(Arg.Any<EncryptedKey>(), Arg.Any<PrivateKey>()); // should decript also with the older version
        fakeCipherV2.Received(0).Decrypt(Arg.Any<EncryptedKey>(), Arg.Any<PrivateKey>()); // and not the new version
    }
}

