using NSubstitute.ReceivedExtensions;
using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.SigabaFiles;
using Sigaba.Crypto;
using Sigaba.Documents;
using Sigaba.Primitives;

namespace Sigaba.App;

public class SigabaAppTest : BaseTest
{
    private readonly ICipher cipher = Substitute.For<ICipher>();
    private readonly IFileCipher fileCipher = Substitute.For<IFileCipher>();
    private readonly IToolSettings toolSettings = Substitute.For<IToolSettings>();
    private readonly ISigabaFileManager settingsManager = Substitute.For<ISigabaFileManager>();
    private readonly IPrivateKeyManager privateKeyManager = Substitute.For<IPrivateKeyManager>();

    private ISigabaApp CreateService()
    {
        settingsManager.LoadAsync(default).ReturnsForAnyArgs(toolSettings);

        return new SigabaApp(Fs, cipher, settingsManager, privateKeyManager, fileCipher);
    }

    // InitAsync

    [Fact]
    public async Task Should_initialize_context()
    {
        var targetDir = Path.Combine("a", "b");
        var publicKey = new PublicKey([1]);
        var privateKey = new PrivateKey([2]);
        cipher.GenerateKeys().Returns((publicKey, privateKey));
        var service = CreateService();

        await service.InitAsync(targetDir);

        cipher.Received().GenerateKeys();
        settingsManager.Received().CreateDefault(publicKey);
        await settingsManager.Received().SaveAsync(Arg.Any<IToolSettings>(), Path.Combine("a", "b", Constants.SigabaFileName));
        await privateKeyManager.Received().SaveAsync(privateKey);
    }

    [Fact]
    public async Task Should_throw_when_initializing_existing_context()
    {
        var targetDir = Path.Combine("a", "b");
        Fs.AddEmptyFile(Path.Combine(targetDir, Constants.SigabaFileName));
        var service = CreateService();

        var action = () => service.InitAsync(targetDir);

        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    // CipherFilesAsync

    [Fact]
    public async Task Should_cipher_files()
    {
        var targetDir = Path.Combine("a", "b");
        Fs.AddEmptyFile(Path.Combine(targetDir, Constants.SigabaFileName));
        var publicKey = new PublicKey([1]);
        toolSettings.PublicKey.Returns(publicKey);
        toolSettings.GetFilesWorkingSet(Fs).Returns(["file1.txt", "file2.txt"]);
        var service = CreateService();

        await service.CipherFilesAsync(targetDir);

        await fileCipher.Received().CipherFile("file1.txt", publicKey, Arg.Any<Predicate<string>>());
        await fileCipher.Received().CipherFile("file2.txt", publicKey, Arg.Any<Predicate<string>>());

    }

    [Fact]
    public async Task Should_throw_when_project_root_not_found()
    {
        var targetDir = Path.Combine("a", "b");
        var service = CreateService();

        var action = () => service.CipherFilesAsync(targetDir);

        await action.Should().ThrowAsync<InvalidOperationException>();
    }

}

