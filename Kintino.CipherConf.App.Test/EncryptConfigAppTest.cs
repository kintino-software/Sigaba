using Kintino.CipherConf.App.Service;
using Kintino.CipherConf.App.TestHelpers;
using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents;
using Kintino.CipherConf.Primitives;
using NSubstitute.ReturnsExtensions;

namespace Kintino.CipherConf.App;

public class EncryptConfigAppTest : BaseTest
{
    private readonly IContext context = Substitute.For<IContext>();
    private readonly IAsymmetricCipher asymmetricCipher = Substitute.For<IAsymmetricCipher>();
    private readonly IFileCipher fileCipher = Substitute.For<IFileCipher>();
    private readonly IFsHelper fsHelper = Substitute.For<IFsHelper>();
    private readonly IContextLoader contextLoader = Substitute.For<IContextLoader>();
    private readonly string currentDir = Path.Combine("a", "b");


    private IEncryptConfigApp CreateService()
    {
        Fs.Directory.SetCurrentDirectory(currentDir);
        contextLoader.LoadContextAsync(currentDir).ReturnsForAnyArgs(context);
        return new EncryptConfigApp(fsHelper, contextLoader, fileCipher, Fs);
    }

    // InitAsync

    [Fact]
    public async Task Should_initialize_context()
    {
        contextLoader.HasContextAsync(currentDir).ReturnsForAnyArgs(false);
        var publicKey = new PublicKey([1]);
        var privateKey = new PrivateKey([2]);
        asymmetricCipher.CreateNewKeyPair().Returns((publicKey, privateKey));
        var service = CreateService();

        await service.InitAsync();

        await contextLoader.Received().HasContextAsync(currentDir);
        await contextLoader.Received().CreateContextAsync(currentDir);
    }

    [Fact]
    public async Task Should_throw_when_initializing_and_context_already_exists()
    {
        contextLoader.HasContextAsync(currentDir).ReturnsForAnyArgs(true);
        var service = CreateService();

        var action = () => service.InitAsync();

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("The app is already initialized.");
        await contextLoader.Received().HasContextAsync(currentDir);
        await contextLoader.DidNotReceive().CreateContextAsync(currentDir);
    }

    // CipherFilesAsync

    [Fact]
    public async Task Should_cipher_files()
    {
        var publicKey = new PublicKey([1]);
        string[] files = ["file1.txt", "file2.txt"];
        context.GetPublicKey().Returns(publicKey);
        context.GetWorkingSetFiles().Returns(files);
        var service = CreateService();

        await service.CipherFilesAsync();

        await contextLoader.Received().LoadContextAsync(currentDir);
        await fileCipher.Received().CipherFile("file1.txt", publicKey, Arg.Any<Predicate<string>>());
        await fileCipher.Received().CipherFile("file2.txt", publicKey, Arg.Any<Predicate<string>>());

    }

    [Fact]
    public async Task Should_throw_when_ciphering_files_without_public_key()
    {
        string[] files = ["file1.txt", "file2.txt"];
        context.GetPublicKey().ReturnsNull();
        context.GetWorkingSetFiles().Returns(files);
        var service = CreateService();

        var action = () => service.CipherFilesAsync();

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Missing public key in context. You cannot cipher files without a public key.");
        await contextLoader.Received().LoadContextAsync(currentDir);
        await fileCipher.Received(0).CipherFile(Arg.Any<string>(), Arg.Any<PublicKey>(), Arg.Any<Predicate<string>>());
    }

}

