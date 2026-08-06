using Sigaba.Primitives;

namespace Sigaba.App.Services.PrivateKeys;

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

        await service.SaveAsync(privateKey);

        Fs.File.Exists("private.key").Should().BeTrue();
    }


    [Fact]
    public async Task Should_overwrite_existing_key_when_saving()
    {
        var service = CreateService();
        var oldKey = CreatePrivateKey(1, 2, 3);
        await service.SaveAsync(oldKey);
        var newKey = CreatePrivateKey(4, 5, 6);

        await service.SaveAsync(newKey);
        var actual = await service.LoadAsync();

        actual.Should().BeEquivalentTo(newKey);
    }

    [Fact]
    public async Task LoadAsync_should_return_privateKey_when_it_exists()
    {
        var service = CreateService();
        var original = CreatePrivateKey();
        await service.SaveAsync(original);

        var actual = await service.LoadAsync();

        actual.Should().BeEquivalentTo(original);
    }

}

