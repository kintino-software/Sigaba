using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Crypto.Services.Ciphers.V1;

public class SymmetricCipherV1Test : BaseTest
{
    private readonly ISymmetricCipher service = new SymmetricCipherV1();

    // Decrypt

    [Fact]
    public void Should_decrypt_encrypted_value()
    {
        var key = service.GenerateNewKey();
        var nonce = service.GenerateNewNonce();
        var plain = new PlainData(GetRandomBytes());
        var secret = service.Encrypt(key, plain, nonce);

        var result = service.Decrypt(key, secret, nonce);

        result.Should().BeEquivalentTo(plain);
    }

    [Fact]
    public void Should_throw_exception_when_decrypting_with_invalid_key()
    {
        var key = service.GenerateNewKey();
        var nonce = service.GenerateNewNonce();
        var plain = new PlainData(GetRandomBytes());
        var secret = service.Encrypt(key, plain, nonce);
        var invalidKey = service.GenerateNewKey();

        var act = () => service.Decrypt(invalidKey, secret, nonce);

        act.Should().Throw<Exception>().WithMessage("Decryption failed.");
    }

    // Encrypt

    [Fact]
    public void Should_generate_same_encrypted_values_for_same_input_and_nonce()
    {
        var key = service.GenerateNewKey();
        var nonce = service.GenerateNewNonce();
        var plain = new PlainData(GetRandomBytes());

        var secret1 = service.Encrypt(key, plain, nonce);
        var secret2 = service.Encrypt(key, plain, nonce);

        secret1.Should().BeEquivalentTo(secret2);
    }

    [Fact]
    public void Should_generate_different_encrypted_values_for_same_input_and_different_nonce()
    {
        var key = service.GenerateNewKey();
        var nonce1 = service.GenerateNewNonce();
        var nonce2 = service.GenerateNewNonce();
        var plain = new PlainData(GetRandomBytes());

        var secret1 = service.Encrypt(key, plain, nonce1);
        var secret2 = service.Encrypt(key, plain, nonce2);

        secret1.Should().NotBeEquivalentTo(secret2);
    }
}

