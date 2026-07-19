using Kintino.CipherConf.IO.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.IO.Implementations;

public class ConcreteContextTest
{
    private static ConcreteContext CreateContext()
    {
        return new ConcreteContext
        {
            SerializablePrivateKey = new SerializablePrivateKey(new PrivateKey(new([1, 2, 3]))),
            SerializablePublicKey = new SerializablePublicKey(new PublicKey(new([4, 5, 6]))),
            SerializableFieldFilter = new SerializableFieldFilter(".*"),
            SerializableFileFilter = new SerializableFileFilter(".*", null),
            SerializableKey = new SerializableKey(new EncryptedKey(new([7, 8, 9])))
        };
    }

    // Serialize

    [Fact]
    public void Should_serialize()
    {
        var context = CreateContext();

        var result = context.Serialize();

        result.Should().NotBeNull();
        result.PrivateKeyStr.Should().NotBeNullOrEmpty();
        result.PublicKeyStr.Should().NotBeNullOrEmpty();
        result.SettingsStr.Should().NotBeNullOrEmpty();
    }

    // Deserialize

    [Fact]
    public void Should_deserialize()
    {
        var original = CreateContext();
        var serialized = original.Serialize();

        var result = ConcreteContext.Deserialize(serialized.PrivateKeyStr, serialized.PublicKeyStr, serialized.SettingsStr);

        result.Should().BeEquivalentTo(original);
    }
}

