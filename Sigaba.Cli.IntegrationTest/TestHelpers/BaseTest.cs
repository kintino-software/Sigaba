using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sigaba.App;
using Sigaba.Primitives.FileSystem;
using Sigaba.Services;
using Spectre.Console.Cli.Testing;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.Cli.IntegrationTest.TestHelpers;

public abstract class BaseTest
{
    protected MockFileSystem Fs { get; } = new();
    protected ITextEditor TextEditor { get; } = Substitute.For<ITextEditor>();
    protected IEnvironmentVariables EnvironmentVariables { get; } = Substitute.For<IEnvironmentVariables>();
    protected CommandAppTester App { get; }

    protected BaseTest()
    {
        App = CreateCommandApp();
    }

    /// <summary>
    /// Initializes the app for testing by running the "init" command with a password.
    /// </summary>
    /// <returns>The initialization data.</returns>
    protected async Task<InitializationData> InitializeAppAsync()
    {
        var cwdDirPath = Fs.NewDirPath("application");
        cwdDirPath.EnsureCreated();
        Fs.Directory.SetCurrentDirectory(cwdDirPath.Path);

        var password = "password";

        var ephemeralApp = CreateCommandApp();
        await ephemeralApp.RunAsync(["init", "-n", "-l", "-p", password]);

        return new InitializationData(password, cwdDirPath.Path);
    }

    public CommandAppTester CreateCommandApp()
    {
        // override services for testing, so we don't mess with the real environment
        // CommandAppTester already injects a TestConsole, so we don't need to override that
        var app = new CommandAppTester(AnsiConsoleSetup.CreateTypeRegistrar(services =>
        {
            services
                // we wont save to disk and create messy temp folders, so we need a mock file system
                .Replace(ServiceDescriptor.Singleton<IFileSystem>(Fs))
                // we wont mess with the real environment variables, so we need a env var mock
                .Replace(ServiceDescriptor.Singleton<IEnvironmentVariables>(EnvironmentVariables))
                // we wont launch a real text editor, so we need a mock
                .Replace(ServiceDescriptor.Singleton<ITextEditor>(TextEditor));

        }));

        // configure app and override some settings if convenient
        app.Configure(cfg => AnsiConsoleSetup.Configure(cfg, null));
        return app;
    }

}
