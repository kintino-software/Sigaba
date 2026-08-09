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
    private readonly ISigabaFileManager sigabaFileManager = Substitute.For<ISigabaFileManager>();
    private readonly IPrivateKeyManager privateKeyManager = Substitute.For<IPrivateKeyManager>();
    private readonly ISigabaFile sigabaFile = Substitute.For<ISigabaFile>();

    private ISigabaApp CreateService()
    {
        sigabaFileManager.LoadAsync(Arg.Any<string>()).Returns(sigabaFile);
        return new SigabaApp(Fs, cipher, sigabaFileManager, privateKeyManager, fileCipher);
    }

    // InitAsync

    [Fact]
    public async Task Should_initialize_context()
    {
        var publicKey = new PublicKey([1]);
        var privateKey = new PrivateKey([2]);
        cipher.GenerateKeys().Returns((publicKey, privateKey));
        var service = CreateService();

        await service.InitAsync(new()
        {
            PrivateKeyPassword = "password",
            SigabaFileOutputDir = "a/b/c",
        });

        cipher.Received().GenerateKeys();
        sigabaFileManager.Received().CreateDefault(publicKey);
        await sigabaFileManager.Received().SaveAsync(Arg.Any<ISigabaFile>(), "a/b/c");
        await privateKeyManager.Received().SaveAsync(Arg.Any<Guid>(), privateKey, "password");
    }

    // CipherFilesAsync

    [Fact]
    public async Task Should_cipher_files()
    {
        var file1 = "a/file1.txt".AsPath();
        var file2 = "a/b/file2.txt".AsPath();
        var rootFolder = "a/".AsPath();
        Fs.AddEmptyFile($"{rootFolder}/{Constants.SigabaFileName}".AsPath());
        sigabaFile.GetTargetFiles(Fs, rootFolder).Returns([file1, file2]);
        var service = CreateService();

        await service.CipherFilesAsync(rootFolder);

        await fileCipher.Received().CipherFile(file1, sigabaFile.PublicKey, sigabaFile.FieldNamePredicate);
        await fileCipher.Received().CipherFile(file2, sigabaFile.PublicKey, sigabaFile.FieldNamePredicate);

    }

}

