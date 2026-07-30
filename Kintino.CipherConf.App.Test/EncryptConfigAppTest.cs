using Kintino.CipherConf.App.Models;
using Kintino.CipherConf.App.Services.FileSystemServices;
using Kintino.CipherConf.App.Services.Serializers;
using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents;
using Kintino.CipherConf.Primitives;
using System.Text.RegularExpressions;

namespace Kintino.CipherConf.App;

public class EncryptConfigAppTest
{
    private readonly IAsymmetricCipher asymmetricCipher = Substitute.For<IAsymmetricCipher>();
    private readonly IFileCipher fileCipher = Substitute.For<IFileCipher>();
    private readonly IFsHelper fsHelper = Substitute.For<IFsHelper>();
    private readonly IContextLoader contextLoader = Substitute.For<IContextLoader>();

    private IEncryptConfigApp CreateService()
    {
        return new EncryptConfigApp(
            asymmetricCipher,
            fsHelper,
            fileCipher,
            contextLoader);
    }

    private static Context FakeContext(PublicKey publicKey, PrivateKey privateKey)
    {
        return new Context()
        {
            SettingsVersion = 1,
            FieldRegex = new Regex(@"foobar"),
            IncludeFileGlob = ["**/*.txt"],
            ExcludeFileGlob = ["**/bin/**", "**/obj/**"],
            PrivateKey = privateKey,
            PublicKey = publicKey,
            AppContextDirectory = "/fake/directory"
        };
    }

    // InitAsync

    [Fact]
    public async Task Should_initialize_context()
    {
        contextLoader.HasContextAsync().ReturnsForAnyArgs(false);
        var publicKey = new PublicKey([1]);
        var privateKey = new PrivateKey([2]);
        asymmetricCipher.CreateNewKeyPair().Returns((publicKey, privateKey));
        var service = CreateService();

        await service.InitAsync();

        await contextLoader.Received().HasContextAsync();
        await contextLoader.Received().CreateContextAsync(publicKey, privateKey);
    }

    [Fact]
    public async Task Should_throw_when_initializing_and_context_already_exists()
    {
        contextLoader.HasContextAsync().ReturnsForAnyArgs(true);
        var service = CreateService();

        var action = () => service.InitAsync();

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("The app is already initialized.");
        await contextLoader.Received().HasContextAsync();
        await contextLoader.Received(0).CreateContextAsync(Arg.Any<PublicKey>(), Arg.Any<PrivateKey>());
    }

    // CipherFilesAsync

    [Fact]
    public async Task Should_cipher_files()
    {
        contextLoader.HasContextAsync().ReturnsForAnyArgs(true);
        var context = FakeContext(publicKey: new([1]), privateKey: null);
        contextLoader.LoadContextAsync().Returns(context);
        fsHelper.Crawl(context.AppContextDirectory, context.IncludeFileGlob, context.ExcludeFileGlob).Returns(["file1.xxx", "file2.xxx"]);
        var service = CreateService();

        await service.CipherFilesAsync();

        await contextLoader.Received().LoadContextAsync();
        await fileCipher.Received().CipherFile("file1.xxx", context.PublicKey, context.FieldRegex.IsMatch);
        await fileCipher.Received().CipherFile("file2.xxx", context.PublicKey, context.FieldRegex.IsMatch);
    }

    [Fact]
    public async Task Should_throw_when_ciphering_files_without_public_key()
    {
        contextLoader.HasContextAsync().ReturnsForAnyArgs(true);
        var context = FakeContext(null, null);
        contextLoader.LoadContextAsync().Returns(context);
        var service = CreateService();

        var action = () => service.CipherFilesAsync();

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Missing public key in context. You cannot cipher files without a public key.");
        await contextLoader.Received().LoadContextAsync();
        await fileCipher.Received(0).CipherFile(Arg.Any<string>(), Arg.Any<PublicKey>(), Arg.Any<Predicate<string>>());
    }

}

