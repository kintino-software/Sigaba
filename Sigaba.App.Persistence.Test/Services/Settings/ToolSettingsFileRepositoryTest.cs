namespace Sigaba.App.Services.Settings;

public class ToolSettingsFileRepositoryTest : BaseTest
{

    private IToolSettingsRepository CreateService()
    {
        return new ToolSettingsFileRepository(Fs);
    }

    [Fact]
    public async Task Should_create_from_any_serialized_settings_version()
    {
        var allVersionTypes = InterfacesInspector.GetAllImplementationsOf<IToolSettings>();
        allVersionTypes.Should().NotBeEmpty("it should have at least one implementation of IToolSettings");
        List<Type> testedInstances = [];
        string[] serializations =
        [
            ToolSettingsV1.CreateDefault().Serialize(),
        ];

        var service = CreateService();

        foreach (var serializedContent in serializations)
        {
            var serialized = serializedContent;
            var fileName = Guid.NewGuid().ToString();
            Fs.AddFile(fileName, serialized);

            var loaded = await service.LoadAsync(fileName);
            testedInstances.Add(loaded.GetType());

            loaded.Should().NotBeNull($"it should load the settings from {fileName}");
        }

        testedInstances.Should().BeEquivalentTo(allVersionTypes, "it should have tested all implementations of IToolSettings");
    }

}

