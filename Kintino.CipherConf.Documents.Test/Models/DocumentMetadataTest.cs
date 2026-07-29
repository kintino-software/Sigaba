namespace Kintino.CipherConf.Documents.Models;

public class DocumentMetadataTest
{
    [Fact]
    public void Should_add_keys_and_return_indexes()
    {
        var obj = new DocumentMetadata([]);

        obj.AddBase64Key("key1", out int index1);
        index1.Should().Be(1);

        obj.AddBase64Key("key2", out int index2);
        index2.Should().Be(2);

        obj.Base64Keys.Should().HaveCount(2);
    }

    [Fact]
    public void Should_not_reuse_removed_indexes()
    {
        var obj = new DocumentMetadata([]);
        obj.AddBase64Key("key1", out _);
        obj.AddBase64Key("key2", out _);

        obj.RemoveBase64Key(1);
        obj.AddBase64Key("key3", out var index);

        index.Should().Be(3);
    }


}

