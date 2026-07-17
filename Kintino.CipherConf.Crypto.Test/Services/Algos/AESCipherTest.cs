using Kintino.CipherConf.Crypto.Primitives;
using Kintino.CipherConf.Crypto.Services.Algos;

namespace Kintino.CipherConf.Crypto.Services.Algos;

public class AESCipherTest : BaseTest
{
    // Decrypt

    [Fact]
    public void Should_decrypt_encrypted_value()
    {
        var key = KeyGenerator.GenerateKey();
        var nonce = NonceGenerator.GenerateNonce();
        var plain = this.GetRandomBytes();
        var secret = AESCipher.Encrypt(plain, key, nonce);

        var result = AESCipher.Decrypt(secret, key, nonce);

        result.Should().BeEquivalentTo(plain);
    }

    [Fact]
    public void Should_throw_exception_when_decrypting_with_invalid_key()
    {
        var key = KeyGenerator.GenerateKey();
        var nonce = NonceGenerator.GenerateNonce();
        var plain = this.GetRandomBytes();
        var secret = AESCipher.Encrypt(plain, key, nonce);
        var invalidKey = KeyGenerator.GenerateKey();

        var act = () => AESCipher.Decrypt(secret, invalidKey, nonce);

        act.Should().Throw<CryptoException>("Decryption failed.");
    }

    // Encrypt

    [Fact]
    public void Should_generate_same_encrypted_values_for_same_input_and_nonce()
    {
        var key = KeyGenerator.GenerateKey();
        var nonce = NonceGenerator.GenerateNonce();
        var plain = this.GetRandomBytes();

        var secret1 = AESCipher.Encrypt(plain, key, nonce);
        var secret2 = AESCipher.Encrypt(plain, key, nonce);

        secret1.Should().BeEquivalentTo(secret2);
    }

    [Fact]
    public void Should_generate_different_encrypted_values_for_same_input_and_different_nonce()
    {
        var key = KeyGenerator.GenerateKey();
        var nonce1 = NonceGenerator.GenerateNonce();
        var nonce2 = NonceGenerator.GenerateNonce();
        var plain = this.GetRandomBytes();

        var secret1 = AESCipher.Encrypt(plain, key, nonce1);
        var secret2 = AESCipher.Encrypt(plain, key, nonce2);

        secret1.Should().NotBeEquivalentTo(secret2);
    }

}

