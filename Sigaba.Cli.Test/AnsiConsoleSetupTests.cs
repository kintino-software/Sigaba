using Sigaba.Cli.Adaptors;
using Spectre.Console.Cli;

namespace Sigaba.Cli;

public class AnsiConsoleSetupTests
{
    private readonly CommandApp app = new();

    // CrateTypeRegistrar

    [Fact]
    public void Should_create_type_registrar()
    {
        var setup = AnsiConsoleSetup.Create();

        setup.TypeRegistrar.Should().NotBeNull();
    }

    // Configure

    [Fact]
    public void Should_configure_app_configurator()
    {
        var setup = AnsiConsoleSetup.Create();

        app.Configure(setup.Configurator);
        var action = () => app.Run([]);

        action.Should().NotThrow();
    }
}

