using Sigaba.Cli.Models;
using Spectre.Console.Cli.Testing;

namespace Sigaba.Cli;

public class AnsiConsoleSetupTests
{
    private CommandAppTester app = new();
    private IGlobalOptions globalOptions = Substitute.For<IGlobalOptions>();

    // CrateTypeRegistrar

    [Fact]
    public void Should_create_type_registrar()
    {
        var typeRegistrar = AnsiConsoleSetup.CreateTypeRegistrar(globalOptions);

        typeRegistrar.Should().NotBeNull();
    }

    // Configure

    [Fact]
    public void Should_configure_app_configurator()
    {
        app.Configure(cfg => cfg.Configure());

        var action = () => app.Run();

        action.Should().NotThrow();
    }
}

