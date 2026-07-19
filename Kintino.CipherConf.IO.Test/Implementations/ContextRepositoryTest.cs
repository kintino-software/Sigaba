using Kintino.CipherConf.IO.Services;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.IO.Implementations;

public class ContextRepositoryTest : BaseTest
{
    private readonly string configFileName = "config.conf";
    private readonly string privateKeyFileName = "private.priv";
    private readonly string publicKeyFileName = "public.pub";
    private readonly IContextSerializer contextSerializer = Substitute.For<IContextSerializer>();

    private IContextRepository CreateService()
    {
        this.Configuration.PrivateKeyFileName.Returns(this.privateKeyFileName);
        this.Configuration.PublicKeyFileName.Returns(this.publicKeyFileName);
        this.Configuration.ToolSettingsFileName.Returns(this.configFileName);
        return new ContextRepository(this.Fs, this.Configuration, this.contextSerializer);
    }

    private static Context CreateContext()
    {
        return new Context
        {
            PrivateKey = new PrivateKey(new([1, 2, 3])),
            PublicKey = new PublicKey(new([4, 5, 6])),
            FieldFilterImpl = new FieldFilter(".*"),
            FileFilterImpl = new FileFilter(".*", ".*"),
            Key = new EncryptedKey(new([7, 8, 9]))
        };
    }

    // CreateContext

    [Fact]
    public async Task Should_save_context()
    {
        var context = CreateContext();
        var service = this.CreateService();

        await service.SaveContext(context, RootPath);

        await contextSerializer.Received(1).SerializeToFileSystem(
            context,
            RootCombine(this.configFileName),
            RootCombine(this.privateKeyFileName),
            RootCombine(this.publicKeyFileName));
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
        var context = CreateContext();
        Fs.AddEmptyFile(RootCombine(this.configFileName));
        Fs.AddEmptyFile(RootCombine(this.privateKeyFileName));
        Fs.AddEmptyFile(RootCombine(this.publicKeyFileName));
        contextSerializer.DeserializeFromFileSystem(default, default, default).ReturnsForAnyArgs(context);


        var result = await service.GetContext(RootPath);

        result.Should().Be(context);
        await contextSerializer.Received(1).DeserializeFromFileSystem(
            RootCombine(this.configFileName),
            RootCombine(this.privateKeyFileName),
            RootCombine(this.publicKeyFileName));
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
        Fs.AddEmptyFile(RootCombine(this.configFileName));
        Fs.AddEmptyFile(RootCombine(this.privateKeyFileName));
        Fs.AddEmptyFile(RootCombine(this.publicKeyFileName));
        var service = this.CreateService();

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

