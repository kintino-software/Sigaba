using Microsoft.Extensions.Logging;
using Sigaba.App.Services.SigabaFiles;
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
        return new PrivateKeyManager(Fs, cipher, locationResolver, logger);
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
        var defaultFilePath = "a/b/file.txt".AsPath();
        locationResolver.GetDefaultFilePath(projectId).Returns(defaultFilePath);
        var service = CreateService();
        var privateKey = CreatePrivateKey(1, 2, 3);
        cipher.EncryptWithPassword(privateKey, "password").Returns(new EncryptedData([4, 4, 4]));

        await service.SaveAsync(projectId, privateKey, "password");

        cipher.Received().EncryptWithPassword(privateKey, "password");
        Fs.File.Exists(defaultFilePath).Should().BeTrue();
        logger.VerifyLog(LogLevel.Information, $"Private key saved to: {defaultFilePath}");
    }

    // LoadAsync

    [Fact]
    public async Task LoadAsync_should_return_privateKey_when_it_exists()
    {
        var projectIdArg = Guid.NewGuid();
        var passwordArg = "password";

        var filePathFromService = "a/b/file.txt".AsPath();
        locationResolver.ResolveCurrentLocation(projectIdArg).Returns(filePathFromService);

        Fs.AddEmptyFile(filePathFromService);

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

