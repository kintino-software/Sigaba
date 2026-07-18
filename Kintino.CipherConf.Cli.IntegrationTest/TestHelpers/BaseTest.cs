using Kintino.CipherConf.Tooling;
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

    protected App CreateApp() => new(Fs, TextEditor);

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