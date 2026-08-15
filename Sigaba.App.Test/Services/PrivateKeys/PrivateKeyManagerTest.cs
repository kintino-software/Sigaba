using Sigaba.Crypto;
using Sigaba.Primitives;

namespace Sigaba.App.Services.PrivateKeys;

public class PrivateKeyManagerTest : BaseTest
{
    private readonly ICipher cipher = Substitute.For<ICipher>();
    private readonly LoggerMock<PrivateKeyManager> logger = new();

    private IPrivateKeyManager CreateService()
    {
        cipher.EncryptWithPassword(default, default).ReturnsForAnyArgs(EncryptedData.Any());
        cipher.DecryptWithPassword(default, default).ReturnsForAnyArgs(PlainData.Any());

        return new PrivateKeyManager(cipher);
    }

    // SaveAsync

    [Fact]
    public async Task Should_save_to_file_system()
    {
        var privateKeyArg = PrivateKey.Any();
        var filePathArg = Fs.NewFilePath("dir/private.key");
        var passwordArg = "password";
        var service = CreateService();

        await service.SaveAsync(privateKeyArg, filePathArg, passwordArg);

        cipher.Received().EncryptWithPassword(privateKeyArg, passwordArg);
        Fs.FileExists(filePathArg.Path).Should().BeTrue();
    }

    [Fact]
    public async Task Should_throw_when_saving_to_existing_file()
    {
        var privateKeyArg = PrivateKey.Any();
        var filePathArg = Fs.NewFilePath("dir/private.key");
        var passwordArg = "password";
        var service = CreateService();

        Fs.AddEmptyFile(filePathArg.Path);
        var action = () => service.SaveAsync(privateKeyArg, filePathArg, passwordArg);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Private key already exists at:*");
    }

    // LoadAsync

    [Fact]
    public async Task Should_load_private_key_from_file_system()
    {
        var filePathArg = Fs.NewFilePath("dir/private.key");
        var passwordArg = "password";
        var service = CreateService();
        await service.SaveAsync(PrivateKey.Any(), filePathArg, passwordArg);

        //

        var actual = await service.LoadAsync(filePathArg, passwordArg);

        //

        actual.Should().NotBeNull();
        cipher.Received().DecryptWithPassword(Arg.Any<EncryptedData>(), passwordArg);


    }

}

