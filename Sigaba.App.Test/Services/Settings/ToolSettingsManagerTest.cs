using Sigaba.App.Services.SigabaFiles;

namespace Sigaba.App.Services.Settings;

public class SigabaFileManagerTest(Fixture fixture) : BaseTest
{
    private static ISigabaFileManager CreateService()
    {
        return new SigabaFileManager();
    }

    // SaveAsync

    [Fact]
    public async Task Should_save_sigaba_file()
    {
        var projectRootArg = Fs.AddDirPath("projectRoot");
        var service = CreateService();
        var instances = fixture.AllImplementationsInstancesOfSigabaFile;

        foreach (var instance in instances)
        {
            var filePath = Fs.NewFilePath($"{Guid.NewGuid()}.json");

            var action = () => service.SaveAsync(instance, projectRootArg);

            await action.Should().NotThrowAsync();
        }
    }

    // LoadAsync

    [Fact]
    public async Task Should_load_sigaba_file()
    {
        var projectRootArg = Fs.AddDirPath("projectRoot");
        var service = CreateService();
        var instances = fixture.AllImplementationsInstancesOfSigabaFile;

        foreach (var instance in instances)
        {
            var saveResult = await service.SaveAsync(instance, projectRootArg);

            var (actualSigabaFile, actualFilePath) = await service.LoadAsync(projectRootArg);

            actualSigabaFile.Should().BeEquivalentTo(instance);
            actualFilePath.Should().Be(saveResult.OutputPath);

        }
    }
}
