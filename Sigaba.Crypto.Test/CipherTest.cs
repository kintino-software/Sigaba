using Microsoft.Extensions.Logging;
using Sigaba.Crypto.Adaptors;
using Sigaba.Crypto.Services.Ciphers;
using Sigaba.Primitives.Crypto;

namespace Sigaba.Crypto;

public class CipherTest
{
    private readonly ILogger<Cipher> logger = Substitute.For<ILogger<Cipher>>();
    private readonly IVersionedCipher fakeCipherV1 = Substitute.For<IVersionedCipher>();
    private readonly IVersionedCipher fakeCipherV2 = Substitute.For<IVersionedCipher>();

    public CipherTest()
    {
        fakeCipherV1.Version.Returns<byte>(1);
        fakeCipherV2.Version.Returns<byte>(2);
        foreach (var cipher in new IVersionedCipher[] { fakeCipherV1, fakeCipherV2 })
        {
            cipher.GenerateKeys().Returns((PublicKey.Any(), PrivateKey.Any()));
            cipher.EncryptWithKey(default, default).ReturnsForAnyArgs(EncryptedData.Any());
            cipher.DecryptWithKey(default, default).ReturnsForAnyArgs(PlainData.Any());
        }
    }

    private ICipher CreateService(IVersionedCipher[] ciphers)
    {
        var service = new Cipher(logger, ciphers);
        return service;
    }

    // Instantiation

    [Fact]
    public void Should_throw_when_instantiating_referencing_ciphers_with_sameVersion()
    {
        var ciper1 = Substitute.For<IVersionedCipher>();
        ciper1.Version.Returns<byte>(1);
        var otherCipherV1 = Substitute.For<IVersionedCipher>();
        otherCipherV1.Version.Returns<byte>(1);

        var action = () => _ = new Cipher(logger, [ciper1, otherCipherV1]);

        action.Should().Throw<ArgumentException>().WithMessage("Multiple asymmetric ciphers found for version 1.");
    }

    [Fact]
    public void Should_throw_when_instantiating_with_no_ciphers()
    {
        var action = () => _ = new Cipher(logger, []);

        action.Should().Throw<ArgumentException>().WithMessage("No asymmetric ciphers are available.");
    }

    // GenerateKeys

    [Fact]
    public void Should_create_key_pairs_with_latest_cipher_implementation()
    {
        var service = CreateService([fakeCipherV1, fakeCipherV2]);

        _ = service.GenerateKeys();

        fakeCipherV2.Received().GenerateKeys();
        fakeCipherV1.DidNotReceive().GenerateKeys();
    }

    [Fact]
    public void Should_tag_public_and_private_keys()
    {
        var service = CreateService([fakeCipherV2]);
        var (publicKey, privateKey) = service.GenerateKeys();

        publicKey.Untag(out var publicKeyVersion);
        privateKey.Untag(out var privateKeyVersion);

        publicKeyVersion.Should().Be(fakeCipherV2.Version);
        privateKeyVersion.Should().Be(fakeCipherV2.Version);
    }

    // Encrypt

    [Fact]
    public void Should_encrypt_with_correct_cipher_implementation()
    {
        var service = CreateService([fakeCipherV1, fakeCipherV2]);
        var (publicKey, _) = service.GenerateKeys();

        _ = service.EncryptWithKey(PlainData.Any(), publicKey);

        fakeCipherV2.Received().EncryptWithKey(Arg.Any<PlainData>(), Arg.Any<PublicKey>());
        fakeCipherV1.DidNotReceive().EncryptWithKey(Arg.Any<PlainData>(), Arg.Any<PublicKey>());
    }

    // Decrypt

    [Fact]
    public void Should_decrypt_with_correct_cipher_version()
    {
        var oldService = CreateService([fakeCipherV1]);
        var (publicKey, privateKey) = oldService.GenerateKeys();
        var oldEncryptedData = oldService.EncryptWithKey(PlainData.Any(), publicKey); // if encrypts with older version

        var newService = CreateService([fakeCipherV1, fakeCipherV2]);

        //

        _ = newService.DecryptWithKey(oldEncryptedData, privateKey);

        //

        fakeCipherV1.Received().EncryptWithKey(Arg.Any<PlainData>(), Arg.Any<PublicKey>());
        fakeCipherV1.Received().DecryptWithKey(oldEncryptedData, Arg.Any<PrivateKey>()); // should decript also with the older version
        fakeCipherV2.DidNotReceive().DecryptWithKey(Arg.Any<EncryptedData>(), Arg.Any<PrivateKey>()); // and not the new version
    }
}

