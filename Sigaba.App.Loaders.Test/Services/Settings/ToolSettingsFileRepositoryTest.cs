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
            Fs.AddFile(Constants.ToolSettingsFileName, serialized);

            var loaded = await service.LoadAsync();
            testedInstances.Add(loaded.GetType());

            loaded.Should().NotBeNull();
        }

        testedInstances.Should().BeEquivalentTo(allVersionTypes, "it should check all implementations of IToolSettings");
    }

}

