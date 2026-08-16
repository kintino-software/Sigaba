namespace Sigaba.Documents.Adaptors;

public class ExtensionsTest
{
  [Fact]
  public void Should_convert_bytes_to_string_and_back()
  {
    var originalString = "Hello, World!";

    var bytes = originalString.ToUTF8Bytes();
    var actualString = bytes.ToUTF8String();

    bytes.Should().NotBeNull();
    bytes.Should().NotBeEmpty();
    actualString.Should().Be(originalString);
  }
}

