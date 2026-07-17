using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.Models;
using Kintino.CipherConf.App.Primitives;
using Kintino.CipherConf.IO.Primitives;

namespace Kintino.CipherConf.IO.Services;

public class ContextRepositoryTest : BaseTest
{
    private readonly IDataSerializer serializer = Substitute.For<IDataSerializer>();
    private readonly string configFileName = "config.conf";
    private readonly string privateKeyFileName = "private.priv";
    private readonly string publicKeyFileName = "public.pub";

    private IContextRepository CreateService()
    {
        this.Configuration.PrivateKeyFileName.Returns(this.privateKeyFileName);
        this.Configuration.PublicKeyFileName.Returns(this.publicKeyFileName);
        this.Configuration.ToolSettingsFileName.Returns(this.configFileName);
        return new ContextRepository(this.Fs, this.Configuration, this.serializer);
    }

    // CreateContext

    [Fact]
    public async Task Should_create_context_with_initialization_data()
    {
        var initData = InitData.FakeInitData();
        var service = this.CreateService();

        await service.CreateContext(initData, RootPath);

        Fs.GetFile(Fs.Path.Combine(RootPath, this.configFileName)).Should().NotBeNull();
        Fs.GetFile(Fs.Path.Combine(RootPath, this.privateKeyFileName)).Should().NotBeNull();
        Fs.GetFile(Fs.Path.Combine(RootPath, this.publicKeyFileName)).Should().NotBeNull();
        serializer.Received(1).SerializeToolSettings(Arg.Any<ToolSettings>());
        serializer.Received(1).SerializePublicKey(Arg.Any<PublicKey>());
        serializer.Received(1).SerializePrivateKey(Arg.Any<PrivateKey>());
    }

    [Theory]
    [InlineData("config.conf")]
    [InlineData("private.priv")]
    [InlineData("public.pub")]
    public async Task Should_throw_when_creating_context_and_files_already_exist(string fileName)
    {
        var initData = InitData.FakeInitData();
        this.Fs.AddFile(Fs.Path.Combine(RootPath, fileName), new MockFileData(string.Empty));
        var service = this.CreateService();


        var action = async () => await service.CreateContext(initData, RootPath);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage($"*{RootPath}*");
    }

    // GetContext

    [Fact]
    public async Task Should_read_context_from_folder()
    {
        serializer.DeserializePublicKey(default).ReturnsForAnyArgs(PublicKey.FakePublicKey());
        serializer.DeserializePrivateKey(default).ReturnsForAnyArgs(PrivateKey.FakePrivateKey());
        serializer.DeserializeToolSettings(default).ReturnsForAnyArgs(ToolSettings.FakeToolSettings());

        var service = this.CreateService();
        var initData = InitData.FakeInitData();
        await service.CreateContext(initData, RootPath);

        var context = await service.GetContext(RootPath);

        context.Should().NotBeNull();
        context.Should().BeOfType<Context>();
        serializer.Received().DeserializePublicKey(Arg.Any<string>());
        serializer.Received().DeserializePrivateKey(Arg.Any<string>());
        serializer.Received().DeserializeToolSettings(Arg.Any<string>());
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
        var initData = InitData.FakeInitData();
        await service.CreateContext(initData, RootPath);

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

