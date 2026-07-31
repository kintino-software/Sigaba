using Kintino.CipherConf.Crypto.Services.Ciphers;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Crypto;

public class SymmetricCipherTest
{
    private readonly IVersionedSymmetricCipher fakeCipherV1 = Substitute.For<IVersionedSymmetricCipher>();
    private readonly IVersionedSymmetricCipher fakeCipherV2 = Substitute.For<IVersionedSymmetricCipher>();
    private readonly Nonce nonce = new([5, 5, 5, 5]);
    private readonly PlainData plainData = new([6, 6, 6, 6]);
    private readonly EncryptedData encryptedData = new([7, 7, 7, 7]);
    private readonly PlainKey key = new([8, 8, 8, 8]);

    public SymmetricCipherTest()
    {
        fakeCipherV1.Version.Returns<byte>(1);
        fakeCipherV2.Version.Returns<byte>(2);
        foreach (var cipher in new[] { fakeCipherV1, fakeCipherV2 })
        {
            cipher.Decrypt(default, default, default).ReturnsForAnyArgs(plainData);
            cipher.Encrypt(default, default, default).ReturnsForAnyArgs(encryptedData);
            cipher.GenerateNewKey().Returns(key);
            cipher.GenerateNewNonce().Returns(nonce);
        }
    }

    private ISymmetricCipher CreateService()
    {
        var service = new SymmetricCipher([fakeCipherV1, fakeCipherV2]);
        return service;
    }

    private ISymmetricCipher CreateService(params IVersionedSymmetricCipher[] ciphers)
    {
        var service = new SymmetricCipher(ciphers);
        return service;
    }

    // GenerateNewKey

    [Fact]
    public void Should_generate_new_key_with_correct_cipher_implementation()
    {
        var service = CreateService();
        _ = service.GenerateNewKey();
        fakeCipherV2.Received(1).GenerateNewKey();
        fakeCipherV1.DidNotReceive().GenerateNewKey();
    }

    // GenerateNewNonce

    [Fact]
    public void Should_generate_new_nonce_with_correct_cipher_implementation()
    {
        var service = CreateService();
        _ = service.GenerateNewNonce();
        fakeCipherV2.Received(1).GenerateNewNonce();
        fakeCipherV1.DidNotReceive().GenerateNewNonce();
    }

    // Encrypt

    [Fact]
    public void Should_encrypt_with_correct_cipher_implementation()
    {
        var service = CreateService();
        _ = service.Encrypt(key, plainData, nonce);
        fakeCipherV2.Received(1).Encrypt(key, plainData, nonce);
        fakeCipherV1.DidNotReceive().Encrypt(key, plainData, nonce);
    }

    [Fact]
    public void Should_tag_encrypted_data()
    {
        var service = CreateService();

        var encryptedData = service.Encrypt(key, plainData, nonce);

        encryptedData.Bytes[0].Should().Be(fakeCipherV2.Version);
    }

    // Decrypt

    [Fact]
    public void Should_decrypt_with_correct_cipher_implementation()
    {
        var oldeService = CreateService(fakeCipherV1); // encrypted with old cipher
        var oldEncryptedData = oldeService.Encrypt(key, plainData, nonce);
        var newService = CreateService(fakeCipherV1, fakeCipherV2);

        _ = newService.Decrypt(key, oldEncryptedData, nonce);

        fakeCipherV1.Received().Decrypt(Arg.Any<PlainKey>(), Arg.Any<EncryptedData>(), Arg.Any<Nonce>()); // so should decrypt with old cipher
        fakeCipherV2.DidNotReceive().Decrypt(Arg.Any<PlainKey>(), Arg.Any<EncryptedData>(), Arg.Any<Nonce>());
    }


}

