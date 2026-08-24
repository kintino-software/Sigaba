using Spectre.Console.Cli;

namespace Sigaba.Cli;

public class AnsiConsoleSetupTests
{
    private readonly CommandApp app = new();

    // CrateTypeRegistrar

    [Fact]
    public void Should_create_type_registrar()
    {
        var typeRegistrar = AnsiConsoleSetup.CreateTypeRegistrar();

        typeRegistrar.Should().NotBeNull();
    }

    // Configure

    [Fact]
    public void Should_configure_app_configurator()
    {
        app.Configure(cfg => cfg.Configure());

        var action = () => app.Run([]);

        action.Should().NotThrow();
    }
}

