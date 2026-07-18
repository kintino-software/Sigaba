
using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.App.Services;

public class FacadeTest
{
    private readonly IAsymmetricCipher AsymmetricCipher = Substitute.For<IAsymmetricCipher>();
    private readonly IRandomKeyGenerator RandomKeyGenerator = Substitute.For<IRandomKeyGenerator>();
    private readonly IContext context = Substitute.For<IContext>();

    private IFacade CreateService()
    {
        return new Facade(this.AsymmetricCipher, this.RandomKeyGenerator);
    }

    // CreateContextKeys

    [Fact]
    public void Should_create_context_keys()
    {
        var publicKey = new PublicKey(new([1, 2, 3]));
        var privateKey = new PrivateKey(new([4, 5, 6]));
        var cryptoBytes = new EncryptedData([7, 8, 9]);
        var randonPlainKey = new PlainKey(new([10, 11, 12]));
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
        var decryptedPlainBytes = new PlainData([1, 2, 3]);
        this.AsymmetricCipher.Decrypt(default, default).ReturnsForAnyArgs(decryptedPlainBytes);
        var service = CreateService();

        var plainKey = service.DecryptKeyFromContext(context);

        plainKey.Should().BeEquivalentTo(new PlainKey(decryptedPlainBytes));

    }
}

