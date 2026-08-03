using Kintino.CipherConf.App.Services.Common;
using Kintino.CipherConf.App.Services.Settings;

namespace Kintino.CipherConf.App.Services.Settings;

public class ToolSettingsFileRepositoryTest : BaseTest
{
    private readonly ToolEnvironment environment;

    public ToolSettingsFileRepositoryTest()
    {
        environment = new ToolEnvironment(Fs);
    }

    private IToolSettingsRepository CreateService()
    {
        return new ToolSettingsFileRepository(Fs);
    }

}

