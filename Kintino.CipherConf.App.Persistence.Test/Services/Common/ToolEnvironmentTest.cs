namespace Kintino.CipherConf.App.Services.Common;

public class ToolEnvironmentTest : BaseTest
{
    [Fact]
    public void Should_get_project_root()
    {
        Fs.AddEmptyFile(Path.Combine("a", "b", "c", Constants.ToolSettingsFileName));
        Fs.Directory.SetCurrentDirectory(Path.Combine("a", "b", "c"));

        var env = new ToolEnvironment(Fs);

        env.GetProjectRootDir().Should().Be(Path.Combine(RootPath, "a", "b", "c"));
    }

    [Fact]
    public void Should_return_null_if_project_root_not_found()
    {
        var env = new ToolEnvironment(Fs);

        env.GetProjectRootDir().Should().BeNull();
    }
}

