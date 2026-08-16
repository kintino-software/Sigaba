using Sigaba.Crypto.Services;
using Sigaba.Crypto.Services.Ciphers;
using Sigaba.Primitives;

namespace Sigaba.Crypto;

public class CipherTest
{
  private readonly IVersionedCipher fakeCipherV1 = Substitute.For<IVersionedCipher>();
  private readonly IVersionedCipher fakeCipherV2 = Substitute.For<IVersionedCipher>();
  private readonly EncryptedData encryptedData = new([9, 9, 9]);
  private readonly PlainData plainData = new([8, 8, 8]);

  public CipherTest()
  {
    var publicKey = new PublicKey([10, 10, 10]);
    var privateKey = new PrivateKey([11, 11, 11]);
    fakeCipherV1.Version.Returns<byte>(1);
    fakeCipherV2.Version.Returns<byte>(2);
    foreach (var cipher in new IVersionedCipher[] { fakeCipherV1, fakeCipherV2 })
    {
      cipher.GenerateKeys().Returns((publicKey, privateKey));
      cipher.EncryptWithKey(default, default).ReturnsForAnyArgs(encryptedData);
      cipher.DecryptWithKey(default, default).ReturnsForAnyArgs(plainData);
    }
  }

  private static ICipher CreateService(params IVersionedCipher[] ciphers)
  {
    var service = new Cipher(ciphers);
    return service;
  }

  // GenerateKeys

  [Fact]
  public void Should_create_key_pairs_with_latest_cipher_implementation()
  {
    var service = CreateService(fakeCipherV1, fakeCipherV2);

    _ = service.GenerateKeys();

    fakeCipherV2.Received(1).GenerateKeys();
  }

  [Fact]
  public void Should_tag_public_and_private_keys()
  {
    var service = CreateService(fakeCipherV2);
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
    var service = CreateService(fakeCipherV2);
    var (publicKey, _) = service.GenerateKeys();

    var result = service.EncryptWithKey(plainData, publicKey);

    fakeCipherV2.Received(1).EncryptWithKey(plainData, Arg.Any<PublicKey>());
  }

  // Decrypt

  [Fact]
  public void Should_decrypt_with_correct_cipher_implementation()
  {
    var oldService = CreateService(fakeCipherV1);
    var (publicKey, privateKey) = oldService.GenerateKeys();
    var oldEncryptedData = oldService.EncryptWithKey(plainData, publicKey); // if encrypts with older version
    var newService = CreateService(fakeCipherV1, fakeCipherV2);

    _ = newService.DecryptWithKey(oldEncryptedData, privateKey);

    fakeCipherV1.Received(1).EncryptWithKey(plainData, Arg.Any<PublicKey>());
    fakeCipherV1.Received(1).DecryptWithKey(oldEncryptedData, Arg.Any<PrivateKey>()); // should decript also with the older version
    fakeCipherV2.Received(0).DecryptWithKey(Arg.Any<EncryptedData>(), Arg.Any<PrivateKey>()); // and not the new version
  }
}

