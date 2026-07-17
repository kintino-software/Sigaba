using Kintino.CipherConf.Documents.Services.Json;
using System.IO.Abstractions.TestingHelpers;

namespace Kintino.CipherConf.Documents.Services;

public class CipherResolverTest
{
    private readonly JsonDocumentCipher jsonDocumentCipher = new(Substitute.For<IValueCipher>());
    private readonly MockFileSystem fs = new();

    private ICipherResolver CreateService(params IDocumentCipher[] availableDocumentCiphers)
    {
        return new CipherResolver(availableDocumentCiphers, fs);
    }

    // Resolve

    [Theory]
    [InlineData("file.json", typeof(JsonDocumentCipher))]
    public void Should_return_correct_document_cipher_instance(string fileName, Type expectedType)
    {
        var service = CreateService(jsonDocumentCipher);

        var result = service.Resolve(fileName);

        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public void Should_return_correct_document_cipher_independently_of_file_extension_case()
    {
        var service = CreateService(jsonDocumentCipher);

        var result = service.Resolve("file.JsOn");

        result.Should().BeOfType<JsonDocumentCipher>();
    }


    [Fact]
    public void Should_throw_when_no_suitable_document_cipher_found()
    {
        var service = CreateService(jsonDocumentCipher);

        var action = () => service.Resolve("file.xxx");

        action.Should().Throw<InvalidOperationException>().WithMessage("File extension '.xxx' is not supported.");
    }
}

