using Sigaba.App.Services.SigabaFiles;

namespace Sigaba.App.Services.Settings;

public class SigabaFileManagerTest(Fixture fixture) : BaseTest
{
    private static ISigabaFileManager CreateService()
    {
        return new SigabaFileManager();
    }

    [Fact]
    public async Task Should_save_sigaba_file()
    {
        var service = CreateService();
        var instances = fixture.AllImplementationsInstancesOfSigabaFile;
        foreach (var instance in instances)
        {
            var filePath = Fs.CreateFile($"{Guid.NewGuid()}.json");

            var action = () => service.SaveAsync(instance, filePath);

            await action.Should().NotThrowAsync();
        }
    }

    [Fact]
    public async Task Should_load_sigaba_file()
    {
        var service = CreateService();
        var instances = fixture.AllImplementationsInstancesOfSigabaFile;
        foreach (var instance in instances)
        {
            var filePath = Fs.CreateFile($"{Guid.NewGuid()}.json");
            await service.SaveAsync(instance, filePath);

            var actual = await service.LoadAsync(filePath);

            actual.Should().BeEquivalentTo(instance);
        }
    }
}
