using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.App.Services.PrivateKeys;

public class PrivateKeyFileRepositoryTest : BaseTest
{
    private IPrivateKeyRepository CreateService()
    {
        return new PrivateKeyFileRepository(Fs);
    }

    private static PrivateKey CreatePrivateKey(params byte[] data)
    {
        return new PrivateKey(data.Length > 0 ? data : [1, 2, 3]);
    }

    [Fact]
    public async Task SaveAsync_should_save_private_key()
    {
        var service = CreateService();
        var privateKey = CreatePrivateKey();

        await service.SaveAsync(privateKey, "private.key");

        Fs.File.Exists("private.key").Should().BeTrue();
    }


    [Fact]
    public async Task Should_overwrite_existing_key_when_saving()
    {
        var service = CreateService();
        var oldKey = CreatePrivateKey(1, 2, 3);
        await service.SaveAsync(oldKey, "private.key");
        var newKey = CreatePrivateKey(4, 5, 6);

        await service.SaveAsync(newKey, "private.key");
        var actual = await service.LoadAsync("private.key");

        actual.Should().BeEquivalentTo(newKey);
    }

    [Fact]
    public async Task LoadAsync_should_return_privateKey_when_it_exists()
    {
        var service = CreateService();
        var original = CreatePrivateKey();
        await service.SaveAsync(original, "private.key");

        var actual = await service.LoadAsync("private.key");

        actual.Should().BeEquivalentTo(original);
    }

}

