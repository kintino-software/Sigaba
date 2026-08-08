using NSubstitute.ReceivedExtensions;
using Sigaba.App.Services.Contexts;
using Sigaba.Crypto;
using Sigaba.Documents;
using Sigaba.Primitives;

namespace Sigaba.App;

public class SigabaAppTest : BaseTest
{
    private readonly ICipher cipher = Substitute.For<ICipher>();
    private readonly IFileCipher fileCipher = Substitute.For<IFileCipher>();
    private readonly IContextLoader contextLoader = Substitute.For<IContextLoader>();
    private readonly Context context = new()
    {
        SigabaRootDir = Path.Combine("a", "b"),
        SigabaFilePath = Path.Combine("a", "b", "sigaba.json"),
        PublicKey = new PublicKey([1]),
        PrivateKey = new PrivateKey([2]),
        FieldFilterPredicate = _ => true,
        WorkingSetFiles = [
            Path.Combine("a", "b", "file1.txt"),
            Path.Combine("a", "b", "file2.txt")
        ]
    };

    private ISigabaApp CreateService()
    {
        contextLoader.LoadContextFromFolderAsync(Arg.Any<string>()).Returns(context);
        return new SigabaApp(Fs, cipher, contextLoader, fileCipher);
    }

    // InitAsync

    [Fact]
    public async Task Should_initialize_context()
    {
        var targetDir = Path.Combine("a", "b");
        var publicKey = new PublicKey([1]);
        var privateKey = new PrivateKey([2]);
        cipher.GenerateKeys().Returns((publicKey, privateKey));
        var service = CreateService();

        await service.InitAsync(targetDir);

        cipher.Received().GenerateKeys();
        await contextLoader.Received().CreateContextAsync(targetDir, publicKey, privateKey);
    }

    // CipherFilesAsync

    [Fact]
    public async Task Should_cipher_files()
    {
        var service = CreateService();

        await service.CipherFilesAsync(Path.Combine("a", "b"));

        await fileCipher.Received().CipherFile(context.WorkingSetFiles.ElementAt(0), context.PublicKey, context.FieldFilterPredicate);
        await fileCipher.Received().CipherFile(context.WorkingSetFiles.ElementAt(1), context.PublicKey, context.FieldFilterPredicate);

    }

}

