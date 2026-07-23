using System.Text;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.TestHelpers;

public abstract class BaseTest
{
    protected FakeSymmetricCipher SymmetricCipher { get; } = new();

    protected static void AssertJsonDocumentIsValid(
        string jsonDocument,
        bool allowTrailingCommas = true,
        JsonCommentHandling commentHandling = JsonCommentHandling.Allow)
    {
        var reader = new Utf8JsonReader(
            Encoding.UTF8.GetBytes(jsonDocument),
            new JsonReaderOptions
            {
                CommentHandling = commentHandling,
                AllowTrailingCommas = allowTrailingCommas,
            });
        while (reader.Read()) { /* just reading to validate */ }
    }
}
