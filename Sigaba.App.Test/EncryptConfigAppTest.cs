using NSubstitute.ReturnsExtensions;
using Sigaba.App.Service;
using Sigaba.App.TestHelpers;
using Sigaba.Crypto;
using Sigaba.Documents;
using Sigaba.Primitives;

namespace Sigaba.App;

public class EncryptConfigAppTest : BaseTest
{
    private readonly IContext context = Substitute.For<IContext>();
    private readonly ICipher cipher = Substitute.For<ICipher>();
    private readonly IFileCipher fileCipher = Substitute.For<IFileCipher>();
    private readonly IFsHelper fsHelper = Substitute.For<IFsHelper>();
    private readonly IContextLoader contextLoader = Substitute.For<IContextLoader>();


    private IEncryptConfigApp CreateService()
    {
        contextLoader.LoadContextAsync(RootDir).ReturnsForAnyArgs(context);
        return new EncryptConfigApp(fsHelper, contextLoader, fileCipher, Fs);
    }

    // InitAsync

    [Fact]
    public async Task Should_initialize_context()
    {
        var publicKey = new PublicKey([1]);
        var privateKey = new PrivateKey([2]);
        cipher.GenerateKeys().Returns((publicKey, privateKey));
        var service = CreateService();

        await service.InitAsync();

        await contextLoader.Received().CreateContextAsync(RootDir);
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

        await contextLoader.Received().LoadContextAsync(RootDir);
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
        await contextLoader.Received().LoadContextAsync(RootDir);
        await fileCipher.Received(0).CipherFile(Arg.Any<string>(), Arg.Any<PublicKey>(), Arg.Any<Predicate<string>>());
    }

}

