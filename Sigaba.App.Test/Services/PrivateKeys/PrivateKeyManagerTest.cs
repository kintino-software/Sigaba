using Sigaba.Crypto;
using Sigaba.Primitives;

namespace Sigaba.App.Services.PrivateKeys;

public class PrivateKeyManagerTest : BaseTest
{
    private readonly ICipher cipher = Substitute.For<ICipher>();
    private readonly IPrivateKeyPathResolver pathResolver = Substitute.For<IPrivateKeyPathResolver>();
    private readonly LoggerMock<PrivateKeyManager> logger = new();


    private IPrivateKeyManager CreateService()
    {
        return new PrivateKeyManager(cipher, pathResolver);
    }

    private void SetupCipher()
    {
        cipher.EncryptWithPassword(default, default).ReturnsForAnyArgs(EncryptedData.Any());
        cipher.DecryptWithPassword(default, default).ReturnsForAnyArgs(PlainData.Any());
    }

    private void SetupPathResolver(out FilePath resolvedPath)
    {
        var path = Fs.NewFilePath("dir/private.key");
        resolvedPath = path;
        pathResolver.GetDefaultSavePath(default).ReturnsForAnyArgs(path);
        pathResolver.GetPossibleLoadingPaths(default, default).ReturnsForAnyArgs([path]);
    }

    // SaveAsync

    [Fact]
    public async Task Should_save_to_file_system()
    {
        var privateKeyArg = PrivateKey.Any();
        var projectIdArg = "projectId";
        var passwordArg = "password";
        var service = CreateService();
        SetupCipher();
        SetupPathResolver(out var filePath);

        await service.SaveAsync(privateKeyArg, projectIdArg, passwordArg);

        cipher.Received().EncryptWithPassword(privateKeyArg, passwordArg);
        filePath.Exists.Should().BeTrue();
    }

    [Fact]
    public async Task Should_throw_when_saving_to_existing_file()
    {
        var privateKeyArg = PrivateKey.Any();
        var projectIdArg = "projectId";
        var passwordArg = "password";
        var service = CreateService();
        SetupCipher();
        SetupPathResolver(out var filePath);

        Fs.AddEmptyFile(filePath.Path);
        var action = () => service.SaveAsync(privateKeyArg, projectIdArg, passwordArg);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Private key already exists at *");
    }

    // LoadAsync

    [Fact]
    public async Task Should_load_private_key_from_file_system()
    {
        var projectRootArg = Fs.NewDirPath("any");
        var projectIdArg = "projectId";
        var passwordArg = "password";
        var service = CreateService();
        SetupCipher();
        SetupPathResolver(out _);
        var expectedPrivateKey = PrivateKey.Any();
        await service.SaveAsync(expectedPrivateKey, projectIdArg, passwordArg);

        //

        var actual = await service.LoadAsync(projectRootArg, projectIdArg, passwordArg);

        //

        actual.Should().NotBeNull();
        pathResolver.Received().GetPossibleLoadingPaths(projectRootArg, projectIdArg);
        cipher.Received().DecryptWithPassword(Arg.Any<EncryptedData>(), passwordArg);

    }

}

