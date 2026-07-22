using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Primitives;
using System.Text;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.TestHelpers;

public abstract class BaseTest
{
    protected ISymmetricCipher SymmetricCipher { get; } = Substitute.For<ISymmetricCipher>();

    protected BaseTest()
    {
        // we add some symmetric cipher behavior so that we can rely on a round-trip encryption and decryption of values
        SymmetricCipher.Encrypt(default, default, default)
            .ReturnsForAnyArgs(ci => new EncryptedData([.. ci.ArgAt<PlainData>(1).Bytes.Reverse()]));
        SymmetricCipher.Decrypt(default, default, default)
            .ReturnsForAnyArgs(ci => new PlainData([.. ci.ArgAt<EncryptedData>(1).Bytes.Reverse()]));
    }

    protected static void AssertJsonDocumentIsValid(string jsonDocument)
    {
        var reader = new Utf8JsonReader(
            Encoding.UTF8.GetBytes(jsonDocument),
            new JsonReaderOptions
            {
                CommentHandling = JsonCommentHandling.Allow,
                AllowTrailingCommas = true
            });
        while (reader.Read()) { }
    }
}
