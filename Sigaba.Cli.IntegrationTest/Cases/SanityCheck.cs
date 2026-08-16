namespace Sigaba.Cli.Cases;

public class SanityCheck : BaseTest
{
    [Fact]
    public async Task Run()
    {
        var action = () => App.RunAsync([]);

        await action.Should().NotThrowAsync();
    }
}
