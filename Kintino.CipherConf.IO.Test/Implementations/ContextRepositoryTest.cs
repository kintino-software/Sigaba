using Kintino.CipherConf.IO.Models;
using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.IO.Implementations;

public class ContextRepositoryTest : BaseTest
{
    private readonly string configFileName = "config.conf";
    private readonly string privateKeyFileName = "private.priv";
    private readonly string publicKeyFileName = "public.pub";

    private IContextRepository CreateService()
    {
        this.Configuration.PrivateKeyFileName.Returns(this.privateKeyFileName);
        this.Configuration.PublicKeyFileName.Returns(this.publicKeyFileName);
        this.Configuration.ToolSettingsFileName.Returns(this.configFileName);
        return new ContextRepository(this.Fs, this.Configuration);
    }

    private static ConcreteContext CreateContext()
    {
        return new ConcreteContext
        {
            PrivateKey = new PrivateKey(new([1, 2, 3])),
            PublicKey = new PublicKey(new([4, 5, 6])),
            FieldFilter = new RegexFilter(".*"),
            FileFilter = new RegexFilter(".*"),
            Key = new EncryptedKey(new([7, 8, 9]))
        };
    }

    // CreateContext

    [Fact]
    public async Task Should_save_context()
    {
        var service = this.CreateService();
        var context = CreateContext();

        await service.SaveContext(context, RootPath);

        Fs.GetFile(Fs.Path.Combine(RootPath, this.configFileName)).Should().NotBeNull();
        Fs.GetFile(Fs.Path.Combine(RootPath, this.privateKeyFileName)).Should().NotBeNull();
        Fs.GetFile(Fs.Path.Combine(RootPath, this.publicKeyFileName)).Should().NotBeNull();
    }

    [Theory]
    [InlineData("config.conf")]
    [InlineData("private.priv")]
    [InlineData("public.pub")]
    public async Task Should_throw_when_creating_context_and_files_already_exist(string fileName)
    {
        this.Fs.AddFile(Fs.Path.Combine(RootPath, fileName), new MockFileData(string.Empty));
        var context = CreateContext();
        var service = this.CreateService();

        var action = async () => await service.SaveContext(context, RootPath);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage($"*{RootPath}*");
    }

    // GetContext

    [Fact]
    public async Task Should_read_context_from_folder()
    {
        var service = this.CreateService();
        var originalContext = CreateContext();
        await service.SaveContext(originalContext, RootPath);

        var result = await service.GetContext(RootPath);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IContext>();
    }

    [Fact]
    public async Task Should_throw_when_getting_context_and_not_initialized()
    {
        var service = this.CreateService();

        var action = async () => await service.GetContext(RootPath);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage($"*{RootPath}*");
    }

    // HasContext

    [Fact]
    public async Task Should_return_true_when_context_exists()
    {
        var service = this.CreateService();
        var context = CreateContext();
        await service.SaveContext(context, RootPath);

        var result = await service.HasContext(RootPath);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Should_return_false_when_context_does_not_exist()
    {
        var service = this.CreateService();

        var result = await service.HasContext(RootPath);

        result.Should().BeFalse();
    }
}

