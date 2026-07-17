namespace EC.Cli.Cases;

public class SanityCheck : BaseTest
{
    [Fact]
    public async Task Run()
    {
        var app = CreateApp();

        var action = () => app.RunAsync();

        await action.Should().NotThrowAsync();
    }
}
