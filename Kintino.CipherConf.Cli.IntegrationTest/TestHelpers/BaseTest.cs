using Kintino.CipherConf.App.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Spectre.Console.Cli;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;

namespace Kintino.CipherConf.Cli.TestHelpers;

public abstract class BaseTest
{
    protected MockFileSystem Fs { get; } = new();
    protected ITextEditor TextEditor { get; } = Substitute.For<ITextEditor>();
    protected static string RootPath { get; }

    static BaseTest()
    {
        RootPath = OperatingSystem.IsWindows() ? @"C:\" : "/";
    }

    protected BaseTest()
    {
        Fs.Directory.SetCurrentDirectory(RootPath);
    }

    protected CliApp CreateApp()
    {
        return new CliApp(
            (services) =>
            {
                services.RemoveAll<ITextEditor>();
                services.RemoveAll<IFileSystem>();
                services.AddSingleton<ITextEditor>(TextEditor);
                services.AddSingleton<IFileSystem>(Fs);
            },
            (configuratior) =>
            {
                configuratior.PropagateExceptions();
            });
    }

    protected async Task<string> GetPropertyFromJsonDocument(string filePath, string propertyName)
    {
        var jsonContent = await Fs.File.ReadAllTextAsync(filePath);
        using var jsonDoc = JsonDocument.Parse(jsonContent);
        if (jsonDoc.RootElement.TryGetProperty(propertyName, out var propertyValue))
        {
            return propertyValue.GetString() ?? string.Empty;
        }
        throw new InvalidOperationException($"Property '{propertyName}' not found in JSON document.");
    }


}