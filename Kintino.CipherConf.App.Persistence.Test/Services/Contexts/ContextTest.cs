using Kintino.CipherConf.App.Services.Settings;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.App.Services.Contexts;

public class ContextTest : BaseTest
{
    private readonly PublicKey publicKey = new([1]);
    private readonly PrivateKey privateKey = new([1]);
    private readonly IToolSettings settings = Substitute.For<IToolSettings>();

    private IContext CreateContext()
    {
        return new Context(RootPath, privateKey, publicKey, settings);
    }

    [Fact]
    public void Should_get_private_key()
    {
        var context = CreateContext();
        context.GetPrivateKey().Should().Be(privateKey);
    }

    [Fact]
    public void Should_get_public_key()
    {
        var context = CreateContext();
        context.GetPublicKey().Should().Be(publicKey);
    }

    [Fact]
    public void Should_get_working_set_files()
    {
        var workingSetFiles = new[] { "file1", "file2" };
        settings.GetFilesWorkingSet(Arg.Any<string>()).Returns(workingSetFiles);
        var context = CreateContext();
        context.GetWorkingSetFiles().Should().BeEquivalentTo(workingSetFiles);
    }

    [Fact]
    public void Should_filter_field_names()
    {
        settings.FieldNamePredicate(Arg.Any<string>()).Returns(true);
        var context = CreateContext();
        context.FieldNameFilter("anyField").Should().BeTrue();
    }
}

