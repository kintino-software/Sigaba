using Sigaba.Primitives;

namespace Sigaba.App.Services.PublicKeys;

public class PublicKeyManagerTest : BaseTest
{
    private IPublicKeyManager CreateService()
    {
        return new PublicKeyManager(Fs);
    }

    private static PublicKey CreatePublicKey(params byte[] data)
    {
        return new PublicKey(data.Length > 0 ? data : [1, 2, 3]);
    }

    [Fact]
    public async Task Should_save_public_key_to_disk()
    {
        var service = CreateService();
        var publicKey = CreatePublicKey();

        await service.SaveAsync(publicKey);

        Fs.File.Exists("public.key").Should().BeTrue();
    }

    [Fact]
    public async Task Should_overwrite_existing_public_key()
    {
        var service = CreateService();
        var oldKey = CreatePublicKey(1, 2, 3);
        await service.SaveAsync(oldKey);
        var newKey = CreatePublicKey(4, 5, 6);

        await service.SaveAsync(newKey);

        var actual = await service.LoadAsync();
        actual.Should().BeEquivalentTo(newKey);
    }

    [Fact]
    public async Task Should_load_public_key_from_disk()
    {
        var service = CreateService();
        var key = CreatePublicKey();
        await service.SaveAsync(key);

        var actual = await service.LoadAsync();

        actual.Should().BeEquivalentTo(key);
    }
}

