namespace Sigaba.Cli.IntegrationTest.Cases;

public class InitTest : BaseTest
{
    private string Cwd => Fs.Directory.GetCurrentDirectory();

    private void AssertInitializationIsCorrect()
    {
        Fs.File.Exists(Fs.Path.Combine(Cwd, "sigaba.json")).Should().BeTrue();
        Fs.AllFiles.Should().ContainSingle(f => f.EndsWith("private.key"));
    }

    // tests

    [Fact]
    public async Task Should_initialize_interactively()
    {
        App.Console.Input.PushTextWithEnter("password"); // enter password
        App.Console.Input.PushTextWithEnter("password"); // confirm password

        var result = await App.RunAsync(["init"]);
        TestContext.Current.TestOutputHelper.WriteLine(App.Console.Output);

        result.ExitCode.Should().Be(0);
        AssertInitializationIsCorrect();
        App.Console.ShouldHaveOutputThatMatches("""
            ^Enter a password to protect the private key: \*+$
            ^Confirm the private key password: \*+$
            ^Sigaba file created at: .*sigaba.json$
            ^Private key created at: .*private.key$
            """);
    }

    [Theory]
    [InlineData("init", "-n", "-p")]
    [InlineData("init", "--non-interactive", "--password")]
    public async Task Should_initialize_non_interactively(string command, string nonInteractiveOption, string passwordOption)
    {
        var result = await App.RunAsync([command, nonInteractiveOption, passwordOption, "password"]);
        TestContext.Current.TestOutputHelper.WriteLine(App.Console.Output);

        result.ExitCode.Should().Be(0);
        AssertInitializationIsCorrect();
        App.Console.ShouldHaveOutputThatMatches("""
            Sigaba file created at: .*sigaba.json
            Private key created at: .*private.key
            """);
    }
}
