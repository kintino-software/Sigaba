using Microsoft.Extensions.DependencyInjection;

namespace Sigaba.Cli.Adaptors;

public class SpectreTypeRegistrarTests
{
    private interface IDummy;
    public class Dummy : IDummy;

    private ServiceCollection services = new();

    private SpectreTypeRegistrar CreateRegistrar() => new(services);

    // Build

    [Fact]
    public void Should_build_resolver()
    {
        var registrar = CreateRegistrar();

        var resolver = registrar.Build();

        resolver.Should().NotBeNull();
    }

    [Fact]
    public void Should_register_service()
    {
        var registrar = CreateRegistrar();
        registrar.Register(typeof(IDummy), typeof(Dummy));

        var resolver = registrar.Build();

        resolver.Resolve(typeof(IDummy)).Should().BeOfType<Dummy>();
    }

    [Fact]
    public void RegisterInstance_should_do_something()
    {
        var registrar = CreateRegistrar();
        var dummyImpl = new Dummy();
        registrar.RegisterInstance(typeof(IDummy), dummyImpl);

        var resolver = registrar.Build();

        resolver.Resolve(typeof(IDummy)).Should().BeSameAs(dummyImpl);
    }

    [Fact]
    public void Should_register_lazy_service()
    {
        var registrar = CreateRegistrar();
        registrar.RegisterLazy(typeof(IDummy), () => new Dummy());

        var resolver = registrar.Build();

        resolver.Resolve(typeof(IDummy)).Should().BeOfType<Dummy>();
    }
}

