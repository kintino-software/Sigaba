using Sigaba.Documents.Services.Env;
using Sigaba.Documents.Services.Json;
using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.Documents.Services;

public class DocumentModelFactoryTests
{
    private readonly MockFileSystem fs = new();

    [Fact]
    public void Should_get_json_document_model()
    {
        var filePath = fs.AddMockFilePath(string.Empty, "a", "b", "c.json");

        var result = DocumentModelFactory.GetDocumentModelByFilePath(filePath);

        result.Should().BeOfType<JsonDocumentModel>();
    }

    [Fact]
    public void Should_get_env_document_model()
    {
        var filePath = fs.AddMockFilePath(string.Empty, "a", "b", ".env");

        var result = DocumentModelFactory.GetDocumentModelByFilePath(filePath);

        result.Should().BeOfType<EnvDocumentModel>();
    }
}

