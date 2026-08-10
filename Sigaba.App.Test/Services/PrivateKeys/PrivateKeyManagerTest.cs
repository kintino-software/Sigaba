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
        var projectId = Guid.NewGuid();
        var filePath = Fs.NewFilePath("a/b/file.txt");
        locationResolver.GetDefaultFilePath(projectId).Returns(filePath);
        var service = CreateService();
        var privateKey = CreatePrivateKey(1, 2, 3);
        cipher.EncryptWithPassword(privateKey, "password").Returns(new EncryptedData([4, 4, 4]));

        await service.SaveAsync(projectId, privateKey, "password");

        cipher.Received().EncryptWithPassword(privateKey, "password");
        filePath.Exists.Should().BeTrue();
        logger.VerifyLog(LogLevel.Information, $"Private key saved to: {filePath.Path}");
    }

    // LoadAsync

    [Fact]
    public async Task LoadAsync_should_return_privateKey_when_it_exists()
    {
        var projectIdArg = Guid.NewGuid();
        var passwordArg = "password";
        var existingPrivateKeyFilePath = Fs.AddFilePath("a/b/file.txt");
        locationResolver.ResolveCurrentLocation(projectIdArg).Returns(existingPrivateKeyFilePath);

        var plainKeyFromService = new PlainData([2]);
        cipher.DecryptWithPassword(Arg.Any<EncryptedData>(), passwordArg).Returns(plainKeyFromService);

        var service = CreateService();
        var expected = new PrivateKey(new PrivateKey(plainKeyFromService));

        //

        var actual = await service.LoadAsync(projectIdArg, passwordArg);

        //

        actual.Should().BeEquivalentTo(expected);
    }

}

