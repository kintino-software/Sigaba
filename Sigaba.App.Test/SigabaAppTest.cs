using NSubstitute.ReceivedExtensions;
using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.SigabaFiles;
using Sigaba.Crypto;
using Sigaba.Documents;
using Sigaba.Primitives;
using Sigaba.Services;

namespace Sigaba.App;

public class SigabaAppTest : BaseTest
{
    private readonly ICipher cipher = Substitute.For<ICipher>();
    private readonly IFileCipher fileCipher = Substitute.For<IFileCipher>();
    private readonly ISigabaFileManager sigabaFileManager = Substitute.For<ISigabaFileManager>();
    private readonly IPrivateKeyManager privateKeyManager = Substitute.For<IPrivateKeyManager>();
    private readonly ISigabaFile sigabaFile = Substitute.For<ISigabaFile>();
    private readonly IEnvironmentVariables env = Substitute.For<IEnvironmentVariables>();

    private ISigabaApp CreateService()
    {
        sigabaFileManager.LoadAsync(Arg.Any<FilePath>()).Returns(sigabaFile);
        cipher.GenerateKeys().Returns((PublicKey.Any(), PrivateKey.Any()));

        return new SigabaApp(Fs, env, cipher, sigabaFileManager, privateKeyManager, fileCipher);
    }

    // InitAsync

    [Fact]
    public async Task Should_initialize_context()
    {
        var optionsArg = new InitializationOptions(Fs.NewDirPath(Fs.Directory.GetCurrentDirectory()), "password");
        var service = CreateService();

        await service.InitAsync(optionsArg);

        cipher.Received().GenerateKeys();
        sigabaFileManager.Received().CreateDefault(Arg.Any<PublicKey>());
        await sigabaFileManager.Received().SaveAsync(Arg.Any<ISigabaFile>(), Arg.Any<FilePath>());

    }

    // CipherFilesAsync

    [Fact]
    public async Task Should_cipher_files()
    {
        var file1 = Fs.NewFilePath("a/file1.txt");
        var file2 = Fs.NewFilePath("a/b/file2.txt");
        var rootFolder = Fs.NewDirPath("a/");
        Fs.AddFilePath(rootFolder.CombineAsFile(Constants.SigabaFileName).Path);
        sigabaFile.GetTargetFiles(rootFolder).Returns([file1, file2]);
        var service = CreateService();

        await service.CipherFilesAsync(rootFolder);

        await fileCipher.Received().CipherFile(file1, sigabaFile.PublicKey, sigabaFile.FieldNamePredicate);
        await fileCipher.Received().CipherFile(file2, sigabaFile.PublicKey, sigabaFile.FieldNamePredicate);

    }

}

