using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.TestHelpers;
using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents;
using Kintino.CipherConf.IO;
using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.App.Services;

public class EncryptConfigAppTest
{
    private readonly IFileOperations FileOperations = Substitute.For<IFileOperations>();
    private readonly ITextEditor TextEditor = Substitute.For<ITextEditor>();
    private readonly ISymmetricCipher SymmetricCipher = Substitute.For<ISymmetricCipher>();
    private readonly IAsymmetricCipher AsymmetricCipher = Substitute.For<IAsymmetricCipher>();
    private readonly IContextFactory ContextFactory = Substitute.For<IContextFactory>();
    private readonly IFileCipher FileCipher = Substitute.For<IFileCipher>();
    private readonly IContextRepository ContextRepository = Substitute.For<IContextRepository>();

    private IEncryptConfigApp CreateService()
    {
        return new EncryptConfigApp(
            this.FileOperations,
            this.AsymmetricCipher,
            this.ContextFactory,
            this.ContextRepository,
            this.FileCipher);
    }

    // Init

    [Fact]
    public async Task Should_create_and_save_context_in_folder()
    {
        this.ContextRepository.HasContext(default).ReturnsForAnyArgs(false);
        var service = CreateService();

        await service.Init("folder");

        await this.ContextRepository.Received().HasContext("folder");

        this.ContextFactory.Received().CreateDefault(Arg.Any<PublicKey>(), Arg.Any<PrivateKey>());
        await this.ContextRepository.Received().SaveContext(Arg.Any<IContext>(), "folder");
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
    public async Task Should_cipher_files()
    {
        var context = new FakeContext();
        this.ContextRepository.GetContext("folder").ReturnsForAnyArgs(context);
        this.FileOperations.GetFilesFromDirectory(default, default).ReturnsForAnyArgs(["file1.txt", "file2.txt"]);

        var service = CreateService();

        //

        await service.CipherFiles("folder");

        //

        await this.FileCipher.Received().CipherFile(
            "file1.txt",
            context.PublicKey,
            context.FieldFilter);

        await this.FileCipher.Received().CipherFile(
            "file2.txt",
            context.PublicKey,
            context.FieldFilter);
    }

}

