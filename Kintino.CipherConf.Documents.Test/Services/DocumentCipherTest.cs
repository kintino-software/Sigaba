using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents.Models;
using Kintino.CipherConf.Documents.TestHelpers;
using Kintino.CipherConf.Primitives;
using NSubstitute.ReceivedExtensions;
using System.Text;

namespace Kintino.CipherConf.Documents.Services;

public class DocumentCipherTest : BaseTest
{
    private readonly INonceGenerator nonceGenerator = Substitute.For<INonceGenerator>();
    private readonly IDocumentModel documentModel = Substitute.For<IDocumentModel>();
    private readonly IDocumentNode node1 = Substitute.For<IDocumentNode>();
    private readonly IDocumentNode node2 = Substitute.For<IDocumentNode>();
    private readonly PlainKey key = new(new([1, 2, 3]));

    private DocumentCipher CreateService()
    {
        nonceGenerator.NewNonce().Returns(new Nonce(new([4, 5, 6])));
        documentModel.GetNodes().Returns([node1, node2]);
        return new DocumentCipher(this.SymmetricCipher, nonceGenerator);
    }

    // Encrypt

    [Fact]
    public void Encrypt_should_encrypt_document()
    {
        node1.Key.Returns("key1");
        node1.Content.Returns("value1");
        node2.Key.Returns("key2");
        node2.Content.Returns("value2");
        var service = CreateService();

        service.Encrypt(documentModel, key, propertyName => true);

        documentModel.Received(1).GetNodes();
        nonceGenerator.Received(2).NewNonce();
        SymmetricCipher.Received(2).Encrypt(key, Arg.Any<PlainData>(), Arg.Any<Nonce>());
        documentModel.Received(1).UpdateNodeContent(node1, Arg.Any<string>());
        documentModel.Received(1).UpdateNodeContent(node2, Arg.Any<string>());
    }

    [Fact]
    public void Encrypt_should_encrypt_filtered_nodes()
    {
        node1.Key.Returns("key1");
        node1.Content.Returns("value1");
        node2.Key.Returns("key2");
        node2.Content.Returns("value2");
        var service = CreateService();

        service.Encrypt(documentModel, key, propertyName => propertyName == "key1");

        documentModel.Received(1).GetNodes();
        nonceGenerator.Received(1).NewNonce();
        SymmetricCipher.Received(1).Encrypt(key, Arg.Any<PlainData>(), Arg.Any<Nonce>());
        documentModel.Received(1).UpdateNodeContent(node1, Arg.Any<string>());
        documentModel.DidNotReceive().UpdateNodeContent(node2, Arg.Any<string>());
    }

    // Decrypt

    [Fact]
    public void Decrypt_should_decrypt_document()
    {
        var pack = new CipherPack(new EncryptedData(Encoding.UTF8.GetBytes("foobar")), new Nonce(new([16, 17])));
        node1.Key.Returns("key1");
        node1.Content.Returns("value1");
        node2.Key.Returns("key2");
        node2.Content.Returns(pack.Pack());
        var service = CreateService();

        service.Decrypt(documentModel, key);

        documentModel.Received(1).GetNodes();
        SymmetricCipher.Received(1).Decrypt(key, Arg.Any<EncryptedData>(), Arg.Any<Nonce>());
        documentModel.Received(1).UpdateNodeContent(node2, Arg.Any<string>());
    }
}

