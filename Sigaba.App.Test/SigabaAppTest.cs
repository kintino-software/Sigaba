using NSubstitute.ReceivedExtensions;
using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.SigabaFiles;
using Sigaba.Crypto;
using Sigaba.Documents;
using Sigaba.Primitives;
using Sigaba.Primitives.FileSystem;

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
        return new SigabaApp(cipher, sigabaFileManager, privateKeyManager, fileCipher);
    }

    private void SetupCipher()
    {
        cipher.GenerateKeys().Returns((PublicKey.Any(), PrivateKey.Any()));
    }

    private void SetupPrivateKeyManager()
    {
        privateKeyManager.LoadAsync(default, default, default).ReturnsForAnyArgs(
            new PrivateKeyLoadResult(PrivateKey.Any(), Fs.NewFilePath("b", "private.key")));
        privateKeyManager.SaveAsync(default, default, default).ReturnsForAnyArgs(
            new PrivateKeySaveResult(Fs.NewFilePath("b", "private.key")));
    }

    private void SetupSigabaFileManager()
    {
        sigabaFileManager.LoadAsync(default).ReturnsForAnyArgs(
            new SigabaFileLoadResult(sigabaFile, Fs.NewFilePath("a", "sigaba.json")));
        sigabaFileManager.SaveAsync(default, default).ReturnsForAnyArgs(
            new SigabaFileSaveResult(Fs.NewFilePath("a", "sigaba.json")));
    }

    // InitAsync

    [Fact]
    public async Task Should_initialize_context()
    {
        var optionsArg = new InitializationOptions(Fs.NewDirPath(Fs.Directory.GetCurrentDirectory()), "password");
        var service = CreateService();
        SetupCipher();
        SetupSigabaFileManager();
        SetupPrivateKeyManager();

        await service.InitAsync(optionsArg);

        cipher.Received().GenerateKeys();
        sigabaFileManager.Received().CreateDefault(Arg.Any<PublicKey>());
        await sigabaFileManager.Received().SaveAsync(Arg.Any<ISigabaFile>(), optionsArg.SigabaFileOutputDir);
        await privateKeyManager.Received().SaveAsync(Arg.Any<PrivateKey>(), Arg.Any<string>(), optionsArg.PrivateKeyPassword);

    }

    // CipherFilesAsync

    [Fact]
    public async Task Should_cipher_files()
    {
        var referenceFolderArg = Fs.NewDirPath("any");
        var service = CreateService();
        FilePath[] files =
            [
                Fs.AddFilePath2(null, "a", "b", "file1.txt"),
                Fs.AddFilePath2(null, "a", "b", "file2.txt"),
            ];
        sigabaFile.GetTargetFiles(default).ReturnsForAnyArgs(files);
        SetupSigabaFileManager();

        await service.CipherFilesAsync(referenceFolderArg);

        await fileCipher.Received().CipherFile(files[0], sigabaFile.PublicKey, sigabaFile.FieldNamePredicate);
        await fileCipher.Received().CipherFile(files[1], sigabaFile.PublicKey, sigabaFile.FieldNamePredicate);

    }

    // DecipherFilesAsync

    [Fact]
    public async Task Should_decipher_files()
    {
        var referenceFolderArg = Fs.NewDirPath("any");
        var passwordArg = "password";
        var service = CreateService();
        FilePath[] files =
            [
                Fs.AddFilePath2(null, "a", "b", "file1.txt"),
                Fs.AddFilePath2(null, "a", "b", "file2.txt"),
            ];
        sigabaFile.GetTargetFiles(default).ReturnsForAnyArgs(files);
        SetupSigabaFileManager();
        SetupPrivateKeyManager();

        await service.DecipherFilesAsync(referenceFolderArg, passwordArg);

        await fileCipher.Received().DecipherFile(files[0], Arg.Any<PrivateKey>());
        await fileCipher.Received().DecipherFile(files[1], Arg.Any<PrivateKey>());
    }

    // EditFileAsync

    [Fact]
    public async Task Should_edit_files()
    {
        var filePathArg = Fs.NewFilePath("a", "b", "file1.txt");
        var textEditorArg = Substitute.For<ITextEditor>();
        var service = CreateService();
        sigabaFile.GetTargetFiles(default).ReturnsForAnyArgs([filePathArg]);
        SetupSigabaFileManager();

        await service.EditFileAsync(textEditorArg, filePathArg);

        await textEditorArg.Received().EditFile(filePathArg);
        await fileCipher.Received().CipherFile(filePathArg, Arg.Any<PublicKey>(), Arg.Any<Predicate<string>>());
    }

    [Fact]
    public async Task Should_throw_when_editing_file_outside_of_a_sigaba_file_context()
    {
        var filePathArg = Fs.NewFilePath("a", "b", "file1.txt");
        var textEditorArg = Substitute.For<ITextEditor>();
        var service = CreateService();
        sigabaFile.GetTargetFiles(default).ReturnsForAnyArgs([Fs.NewFilePath("a", "b", "other-file.txt")]);
        SetupSigabaFileManager();

        var action = () => service.EditFileAsync(textEditorArg, filePathArg);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"The file '{filePathArg}' is not part of Sigaba target files*");
    }

}

