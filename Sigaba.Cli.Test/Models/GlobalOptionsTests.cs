namespace Sigaba.Cli.Models;

public class GlobalOptionsTests
{
    private IGlobalOptions CreateOptions()
    {
        return new GlobalOptions();
    }

    [Fact]
    public void Should_set_verbosity_level()
    {
        var options = CreateOptions();
        var expected = options.Verbosity == VerbosityLevel.Normal ? VerbosityLevel.Detailed : VerbosityLevel.Normal;

        options.SetVerbosity(expected);

        options.Verbosity.Should().Be(expected);
    }

    [Fact]
    public void Should_set_verbosity_level_with_thread_safety()
    {
        var options = CreateOptions();
        options.SetVerbosity(VerbosityLevel.Normal);

        var t1 = new Thread(() =>
        {
            for (int i = 0; i < 2000; i++) options.SetVerbosity(VerbosityLevel.Detailed);
        });
        var t2 = new Thread(() =>
        {
            for (int i = 0; i < 2000; i++) options.SetVerbosity(VerbosityLevel.Quiet);
        });

        //

        t1.Start();
        t2.Start();
        t1.Join();
        t2.Join();

        //

        options.Verbosity.Should().BeOneOf(VerbosityLevel.Detailed, VerbosityLevel.Quiet);
    }
}

