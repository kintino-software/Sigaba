using Kintino.CipherConf.App.Services.PrivateKeys;
using Kintino.CipherConf.App.Services.PublicKeys;
using Kintino.CipherConf.App.Services.Settings;
using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Primitives;
using NSubstitute.ReturnsExtensions;

namespace Kintino.CipherConf.App.Services.Contexts;

public class ContextLoaderTest : BaseTest
{
    private readonly IToolSettingsRepository settingsRepository = Substitute.For<IToolSettingsRepository>();
    private readonly IPublicKeyRepository publicKeyRepository = Substitute.For<IPublicKeyRepository>();
    private readonly IPrivateKeyRepository privateKeyRepository = Substitute.For<IPrivateKeyRepository>();
    private readonly IAsymmetricCipher asymmetricCipher = Substitute.For<IAsymmetricCipher>();

    private IContextLoader CreateService()
    {
        return new ContextLoader(settingsRepository, publicKeyRepository, privateKeyRepository, asymmetricCipher, Fs);
    }

    // CreateContextAsync

    [Fact]
    public async Task Should_create_context()
    {
        var targetDir = FromRoot("a", "b");
        Fs.AddDirectory(targetDir);
        settingsRepository.LoadAsync(RootPath).ReturnsNull();
        var publicKey = new PublicKey([1]);
        var privateKey = new PrivateKey([2]);
        asymmetricCipher.CreateNewKeyPair().Returns((publicKey, privateKey));
        var service = CreateService();

        await service.CreateContextAsync(targetDir);

        asymmetricCipher.Received().CreateNewKeyPair();
        await settingsRepository.Received().SaveDefaultAsync(Path.Combine(targetDir, Constants.ToolSettingsFileName));
        await publicKeyRepository.Received().SaveAsync(publicKey, Path.Combine(targetDir, Constants.PublicKeyFileName));
        await privateKeyRepository.Received().SaveAsync(privateKey, Path.Combine(targetDir, Constants.PrivateKeyFileName));
    }

    [Fact]
    public async Task Should_throw_if_creating_context_where_it_already_exists()
    {
        Fs.AddEmptyFile(FromRoot(Constants.ToolSettingsFileName));
        var service = CreateService();

        var action = () => service.CreateContextAsync(RootPath);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("A context already exists in this folder.");
    }

    // LoadContextAsync

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public async Task Should_load_existing_context_with_or_without_keys(bool hasPublicKey, bool hasPrivateKey)
    {
        var publicKey = hasPublicKey ? new PublicKey([1]) : null;
        var privateKey = hasPrivateKey ? new PrivateKey([2]) : null;
        var settings = Substitute.For<IToolSettings>();
        settingsRepository.LoadAsync(default).Returns(settings);
        publicKeyRepository.LoadAsync(default).ReturnsForAnyArgs(publicKey);
        privateKeyRepository.LoadAsync(default).ReturnsForAnyArgs(privateKey);
        var service = CreateService();

        var result = await service.LoadContextAsync(RootPath);

        await publicKeyRepository.Received().LoadAsync(Path.Combine(RootPath, Constants.PublicKeyFileName));
        await privateKeyRepository.Received().LoadAsync(Path.Combine(RootPath, Constants.PrivateKeyFileName));
        await settingsRepository.Received().LoadAsync(Path.Combine(RootPath, Constants.ToolSettingsFileName));
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_throw_if_settings_not_found()
    {
        settingsRepository.LoadAsync(default).ReturnsNullForAnyArgs();
        var service = CreateService();

        var action = () => service.LoadContextAsync(RootPath);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("No context in this folder. You have to initialize it first.");
    }
}

