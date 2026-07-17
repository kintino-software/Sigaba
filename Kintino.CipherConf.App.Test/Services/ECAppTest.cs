using Kintino.CipherConf.App.Models;
using Kintino.CipherConf.App.Primitives;

namespace Kintino.CipherConf.App.Services;

public class ECAppTest : BaseTest
{
    private IECApp CreateService()
    {
        return new ECApp(
            this.FileOperations,
            this.TextEditor,
            this.AsymmetricCipher,
            this.SymmetricCipher,
            this.ContextRepository,
            this.FileCipher,
            this.Facade);
    }

    // Init

    [Fact]
    public async Task Should_initialize()
    {
        var privateKey = PrivateKey.FakePrivateKey();
        var publicKey = PublicKey.FakePublicKey();
        var cryptoKey = CryptoKey.FakeCryptoKey();
        var expectredInitData = new InitData()
        {
            FileRegex = @"^appsettings.*\.json$",
            PropertyRegex = @"_secret$",
            FolderPath = "folder",
            PrivateKey = privateKey,
            PublicKey = publicKey,
            Key = cryptoKey,
        };

        this.ContextRepository.HasContext(default).ReturnsForAnyArgs(false);
        this.Facade.CreateContextKeys().Returns((publicKey, privateKey, cryptoKey));
        var service = CreateService();

        //

        await service.Init("folder");

        //

        await this.ContextRepository.Received().CreateContext(expectredInitData, "folder");
    }

    [Fact]
    public async Task Should_throw_when_initializing_a_folder_already_initialized()
    {
        this.ContextRepository.HasContext(default).ReturnsForAnyArgs(true);
        var service = CreateService();

        var action = async () => await service.Init("folder");

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("The folder 'folder' is already initialized.");
    }

    // CipherFiles

    [Fact]
    public async Task Should_cipher_files_in_folder()
    {
        var context = Context.FakeContext();
        var plainKey = PlainKey.FakePlainKey();
        this.ContextRepository.GetContext(default).ReturnsForAnyArgs(context);
        this.Facade.DecryptKeyFromContext(context).Returns(plainKey);
        this.FileOperations.GetFilesFromDirectory(default, default).ReturnsForAnyArgs(["file1.txt", "file2.txt"]);
        var service = CreateService();

        //

        await service.CipherFiles("targetFolder");

        //

        await this.FileCipher.Received().CipherFile(
            "file1.txt",
            Arg.Any<PlainKey>(),
            this.SymmetricCipher,
            context.PropertyRegex);

        await this.FileCipher.Received().CipherFile(
            "file2.txt",
            Arg.Any<PlainKey>(),
            this.SymmetricCipher,
            context.PropertyRegex);
    }

    // DecipherFiles



    // EditFile





}

