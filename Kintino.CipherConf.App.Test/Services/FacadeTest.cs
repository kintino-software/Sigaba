
using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Primitives;
using Kintino.CipherConfig;

namespace Kintino.CipherConf.App.Services;

public class FacadeTest
{
    private readonly IAsymmetricCipher AsymmetricCipher = Substitute.For<IAsymmetricCipher>();
    private readonly IRandomKeyGenerator RandomKeyGenerator = Substitute.For<IRandomKeyGenerator>();

    private IFacade CreateService()
    {
        return new Facade(this.AsymmetricCipher, this.RandomKeyGenerator);
    }

    // CreateContextKeys

    [Fact]
    public void Should_create_context_keys()
    {
        var publicKey = PublicKey.FakePublicKey();
        var privateKey = PrivateKey.FakePrivateKey();
        var cryptoBytes = EncryptedData.FakeEncryptedData();
        var randonPlainKey = PlainKey.FakePlainKey();
        this.AsymmetricCipher.CreateNewKeyPair().Returns((publicKey, privateKey));
        this.RandomKeyGenerator.GenerateNewKey().Returns(randonPlainKey);
        this.AsymmetricCipher.Encrypt(default, default).ReturnsForAnyArgs(cryptoBytes);
        var service = CreateService();

        var (actualPublicKey, actualPrivateKey, actualCryptoBytes) = service.CreateContextKeys();

        actualPublicKey.Should().BeEquivalentTo(publicKey);
        actualPrivateKey.Should().BeEquivalentTo(privateKey);
        actualCryptoBytes.Should().BeEquivalentTo(cryptoBytes);
    }

    // DecryptKeyFromContext

    [Fact]
    public void Should_get_plain_key_from_context()
    {
        var decryptedPlainBytes = PlainData.FakePlainData();
        var context = new FakeContext();
        this.AsymmetricCipher.Decrypt(default, default).ReturnsForAnyArgs(decryptedPlainBytes);
        var service = CreateService();

        var plainKey = service.DecryptKeyFromContext(context);

        plainKey.Should().BeEquivalentTo(new PlainKey(decryptedPlainBytes));

    }
}

