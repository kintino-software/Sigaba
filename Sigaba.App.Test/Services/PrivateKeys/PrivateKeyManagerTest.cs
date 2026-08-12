using Microsoft.Extensions.Logging;
using Sigaba.Crypto;
using Sigaba.Primitives;

namespace Sigaba.App.Services.PrivateKeys;

public class PrivateKeyManagerTest : BaseTest
{
    private readonly ICipher cipher = Substitute.For<ICipher>();
    private readonly LoggerMock<PrivateKeyManager> logger = new();
    private readonly IPrivateKeyLocationResolver locationResolver = Substitute.For<IPrivateKeyLocationResolver>();

    private IPrivateKeyManager CreateService()
    {
        return new PrivateKeyManager(cipher, locationResolver, logger);
    }

    private static PrivateKey CreatePrivateKey(params byte[] data)
    {
        return new PrivateKey(data);
    }

    // SaveAsync

    [Fact]
    public async Task SaveAsync_should_save_private_key_default_folder()
    {
        var projectIdArg = Guid.NewGuid();
        var privateKeyArg = PrivateKey.Any();
        var passwordArg = "password";
        DirPath customLocationArg = Fs.NewDirPath("dir");
        var returnedFilePath = Fs.NewFilePath("private.key");
        var service = CreateService();
        cipher.EncryptWithPassword(privateKeyArg, passwordArg).Returns(EncryptedData.Any());

        await service.SaveAsync(projectIdArg, privateKeyArg, passwordArg, customLocationArg);

        cipher.Received().EncryptWithPassword(privateKeyArg, passwordArg);
        returnedFilePath.Exists.Should().BeTrue();
        logger.VerifyLog(LogLevel.Information, $"Private key saved to: {returnedFilePath.Path}");
    }

    // LoadAsync

    [Fact]
    public async Task LoadAsync_should_return_privateKey_when_it_exists()
    {
        locationResolver.GetLoadPath(default, default).ReturnsForAnyArgs(Fs.NewFilePath("dir/private.key"));
        cipher.DecryptWithPassword(default, default).Returns(PlainData.Any());
        var service = CreateService();

        //

        var actual = await service.LoadAsync(Guid.NewGuid(), "password", Fs.NewDirPath("dir"));

        //

        actual.Should().NotBeNull();

    }

}

