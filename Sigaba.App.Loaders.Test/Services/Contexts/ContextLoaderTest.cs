using NSubstitute.ReturnsExtensions;
using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.PublicKeys;
using Sigaba.App.Services.Settings;
using Sigaba.Crypto;
using Sigaba.Primitives;

namespace Sigaba.App.Services.Contexts;

public class ContextLoaderTest : BaseTest
{
    private readonly IToolSettingsRepository settingsRepository = Substitute.For<IToolSettingsRepository>();
    private readonly IPublicKeyRepository publicKeyRepository = Substitute.For<IPublicKeyRepository>();
    private readonly IPrivateKeyRepository privateKeyRepository = Substitute.For<IPrivateKeyRepository>();
    private readonly ICipher cipher = Substitute.For<ICipher>();

    private IContextLoader CreateService()
    {
        return new ContextLoader(settingsRepository, publicKeyRepository, privateKeyRepository, cipher, Fs);
    }

    // CreateContextAsync

    [Fact]
    public async Task Should_create_context()
    {
        var targetDir = FromRoot("a", "b");
        Fs.AddDirectory(targetDir);
        settingsRepository.LoadAsync().ReturnsNull();
        var publicKey = new PublicKey([1]);
        var privateKey = new PrivateKey([2]);
        cipher.GenerateKeys().Returns((publicKey, privateKey));
        var service = CreateService();

        await service.CreateContextAsync();

        cipher.Received().GenerateKeys();
        await settingsRepository.Received().SaveDefaultAsync();
        await publicKeyRepository.Received().SaveAsync(publicKey);
        await privateKeyRepository.Received().SaveAsync(privateKey);
    }

    [Fact]
    public async Task Should_throw_if_creating_context_where_it_already_exists()
    {
        settingsRepository.ExistsAsync().Returns(true);
        Fs.AddEmptyFile(FromRoot(Constants.ToolSettingsFileName));
        var service = CreateService();

        var action = () => service.CreateContextAsync();

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
        settingsRepository.ExistsAsync().Returns(true);
        settingsRepository.LoadAsync().Returns(settings);
        publicKeyRepository.LoadAsync().ReturnsForAnyArgs(publicKey);
        privateKeyRepository.LoadAsync().ReturnsForAnyArgs(privateKey);
        var service = CreateService();

        var result = await service.LoadContextAsync();

        await publicKeyRepository.Received().LoadAsync();
        await privateKeyRepository.Received().LoadAsync();
        await settingsRepository.Received().LoadAsync();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_throw_if_settings_not_found()
    {
        settingsRepository.LoadAsync().ReturnsNullForAnyArgs();
        var service = CreateService();

        var action = () => service.LoadContextAsync();

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("No context in this folder. You have to initialize it first.");
    }
}

