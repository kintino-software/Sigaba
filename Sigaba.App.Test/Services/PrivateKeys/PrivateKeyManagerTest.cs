using Sigaba.Crypto;
using Sigaba.Primitives.Crypto;
using Sigaba.Primitives.FileSystem;
using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.App.Services.PrivateKeys;

public class PrivateKeyManagerTest : BaseTest
{
    private readonly ICipher cipher = Substitute.For<ICipher>();
    private readonly IPrivateKeyPathResolver pathResolver = Substitute.For<IPrivateKeyPathResolver>();
    private readonly MockFileSystem fs = new();

    private IPrivateKeyManager CreateService()
    {
        return new PrivateKeyManager(fs, cipher, pathResolver, CreateLogger<PrivateKeyManager>());
    }

    private void SetupCipher()
    {
        cipher.EncryptWithPassword(default, default).ReturnsForAnyArgs(EncryptedData.Any());
        cipher.DecryptWithPassword(default, default).ReturnsForAnyArgs(PlainData.Any());
    }

    private void SetupPathResolver(out FilePath resolvedPath)
    {
        var path = new FilePath(fs.Directory.GetCurrentDirectory(), "dir", "private.key");
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

        cipher.Received().EncryptWithPassword(new PlainData(privateKeyArg.Bytes), passwordArg);
        fs.FileExists(filePath).Should().BeTrue();
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

        fs.AddEmptyFile(filePath);
        var action = () => service.SaveAsync(privateKeyArg, projectIdArg, passwordArg);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage($"*already exists*");
    }

    // LoadAsync

    [Fact]
    public async Task Should_load_private_key_from_file_system()
    {
        var projectRootArg = new DirPath("any");
        var projectIdArg = "projectId";
        var passwordArg = "password";
        var service = CreateService();
        var expectedPrivateKey = PrivateKey.Any();
        SetupCipher();
        SetupPathResolver(out _);
        await service.SaveAsync(expectedPrivateKey, projectIdArg, passwordArg);

        //

        var actual = await service.LoadAsync(projectRootArg, projectIdArg, passwordArg);

        //

        actual.Should().NotBeNull();
        pathResolver.Received().GetPossibleLoadingPaths(projectRootArg, projectIdArg);
        cipher.Received().DecryptWithPassword(Arg.Any<EncryptedData>(), passwordArg);

    }

}

