using NSubstitute.ReceivedExtensions;
using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.SigabaFiles;
using Sigaba.Primitives;

namespace Sigaba.App.Services.Contexts;

public class ContextLoaderTest : BaseTest
{
    private readonly ISigabaFileManager sigabaFileManager = Substitute.For<ISigabaFileManager>();
    private readonly IPrivateKeyManager privateKeyManager = Substitute.For<IPrivateKeyManager>();

    private IContextLoader CreateService()
    {
        return new ContextLoader(Fs, sigabaFileManager, privateKeyManager);
    }

    private ISigabaFile CreateSigabaFile()
    {
        var sigabaFile = Substitute.For<ISigabaFile>();
        sigabaFile.PublicKey.Returns(new PublicKey([1, 2, 3]));
        sigabaFile.GetTargetFiles(Fs, Arg.Any<string>()).Returns(new List<string> { "file1.txt", "file2.txt" });
        return sigabaFile;
    }

    // CreateContextAsync

    [Fact]
    public async Task Should_create_context()
    {
        var publicKey = new PublicKey([1, 2, 3]);
        var privateKey = new PrivateKey([4, 5, 6]);
        var targetDir = Fs.Path.Combine("a", "b");
        var service = CreateService();

        await service.CreateContextAsync(targetDir, publicKey, privateKey);

        sigabaFileManager.Received().CreateDefault(publicKey);
        // sigaba file must be saved on the target folder
        await sigabaFileManager.Received().SaveAsync(Arg.Any<ISigabaFile>(), Fs.Path.Combine(targetDir, Constants.SigabaFileName));
        // private key must be saved in the system folder
        await privateKeyManager.Received().SaveAsync(privateKey, Fs.Path.Combine(Constants.SigabaSystemFolderPath, Constants.PrivateKeyFileName));
    }

    // LoadContextFromFolderAsync

    [Theory]
    [InlineData("a/b", "a/b/sigaba.json", "a/b")]
    [InlineData("a/b/c", "a/b/sigaba.json", "a/b")]
    [InlineData("a/b/c/d/e/f/g/h", "a/b/sigaba.json", "a/b")]
    public async Task Should_load_context_from_any_folder_in_the_folder_hierarchy(
        string inputPath,
        string sigabaFilePath,
        string expectedRootDir)
    {
        Fs.AddDirectory(inputPath.AsPath());
        Fs.AddEmptyFile(sigabaFilePath.AsPath());
        var publicKey = new PublicKey([1, 2, 3]);
        var privateKey = new PrivateKey([4, 5, 6]);

        var sigabaFile = CreateSigabaFile();
        sigabaFileManager.LoadAsync(sigabaFilePath.AsPath()).Returns(sigabaFile);
        privateKeyManager.LoadAsync(default).ReturnsForAnyArgs(privateKey);
        var service = CreateService();

        //

        var context = await service.LoadContextFromFolderAsync(inputPath.AsPath());

        //

        context.Should().BeEquivalentTo(new Context()
        {
            FieldFilterPredicate = sigabaFile.FieldNamePredicate,
            PublicKey = sigabaFile.PublicKey,
            PrivateKey = privateKey,
            SigabaFilePath = sigabaFilePath.AsPath(),
            SigabaRootDir = expectedRootDir.AsPath(),
            WorkingSetFiles = sigabaFile.GetTargetFiles(Fs, inputPath.AsPath()),
        });
    }
}

