using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sigaba.App;
using Sigaba.Cli.Adaptors;
using Sigaba.Services;
using Spectre.Console;
using Spectre.Console.Cli.Testing;
using Spectre.Console.Testing;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.Cli.TestHelpers;

public abstract class BaseTest
{
  protected MockFileSystem Fs { get; } = new();
  protected ITextEditor TextEditor { get; } = Substitute.For<ITextEditor>();
  protected FakeEnvironmentVariables EnvironmentVariables { get; } = new();
  protected TestConsole AnsiConsole { get; } = new();
  protected CommandAppTester App { get; }

  protected BaseTest()
  {
    App = new CommandAppTester(CommandAppSetup.CreateTypeRegistrar(services =>
    {
      services
              .Replace(ServiceDescriptor.Singleton<IAnsiConsole>(AnsiConsole))
              .Replace(ServiceDescriptor.Singleton<IFileSystem>(Fs))
              .Replace(ServiceDescriptor.Singleton<IEnvironmentVariables>(EnvironmentVariables))
              .Replace(ServiceDescriptor.Singleton<ITextEditor>(TextEditor));

    }));
    App.Configure(cfg => CommandAppSetup.Configurator(cfg, null));
  }
}
