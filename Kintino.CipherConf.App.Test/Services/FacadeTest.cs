
using Kintino.CipherConf.App.TestHelpers;
using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Primitives;
using NSubstitute.ReceivedExtensions;

namespace Kintino.CipherConf.App.Services;

public class FacadeTest
{
    private readonly IAsymmetricCipher asymmetricCipher = Substitute.For<IAsymmetricCipher>();
    private readonly IRandomKeyGenerator randomKeyGenerator = Substitute.For<IRandomKeyGenerator>();

    private IFacade CreateService()
    {
        return new Facade(this.asymmetricCipher, this.randomKeyGenerator);
    }

    // CreateContextKeys

    [Fact]
    public void Should_create_context_keys()
    {
        var publicKey = new PublicKey(new([1, 2, 3]));
        var privateKey = new PrivateKey(new([4, 5, 6]));
        this.asymmetricCipher.CreateNewKeyPair().Returns((publicKey, privateKey));

        var plainKey = new PlainKey(new([10, 11, 12]));
        this.randomKeyGenerator.GenerateNewKey().Returns(plainKey);

        var encryptedData = new EncryptedData([7, 8, 9]);
        this.asymmetricCipher.Encrypt(Arg.Any<PlainData>(), publicKey).ReturnsForAnyArgs(encryptedData);

        var service = CreateService();

        //

        var (actualPublicKey, actualPrivateKey, actualCryptoBytes) = service.CreateContextKeys();

        //

        asymmetricCipher.Received(1).CreateNewKeyPair();
        randomKeyGenerator.Received(1).GenerateNewKey();
        asymmetricCipher.Received(1).Encrypt(Arg.Is<PlainData>(x => x.Bytes.SequenceEqual(plainKey.Bytes)), publicKey);

        actualPublicKey.Should().BeEquivalentTo(publicKey);
        actualPrivateKey.Should().BeEquivalentTo(privateKey);
        actualCryptoBytes.Should().BeEquivalentTo(encryptedData);
    }

    // DecryptKeyFromContext

    [Fact]
    public void Should_get_plain_key_from_context()
    {
        var context = new FakeContext();
        var decryptedPlainBytes = new PlainData([1, 2, 3]);
        this.asymmetricCipher.Decrypt(default, default).ReturnsForAnyArgs(decryptedPlainBytes);
        var service = CreateService();

        var plainKey = service.DecryptKeyFromContext(context);

        plainKey.Should().BeEquivalentTo(new PlainKey(decryptedPlainBytes));

    }
}

