using Sigaba.App.Services.SigabaFiles;
using Sigaba.App.Services.SigabaFiles.V1;
using Sigaba.Primitives;

namespace Sigaba.App.Services.Settings.V1;

public class ToolSettingsV1Test : BaseTest
{
    private readonly PublicKey publicKey = new([1, 2, 3, 4]);
    private readonly Guid projectId = Guid.NewGuid();

    private ISigabaFile CreateModel(string fieldRegex = null, string[] includeGlob = null, string[] excludeGlob = null)
    {
        return fieldRegex == null && includeGlob == null && excludeGlob == null
            ? SigabaFileV1.CreateDefault(publicKey)
            : new SigabaFileV1(
                fieldRegex ?? @".*",
                includeGlob ?? [],
                excludeGlob ?? [],
                projectId,
                publicKey);
    }

    // Serialize

    [Fact]
    public void Should_serialize_to_json()
    {
        var model = SigabaFileV1.CreateDefault(publicKey);
        var action = () => model.Serialize();
        action.Should().NotThrow();
    }

    // Deserialize

    [Fact]
    public void Should_deserialize_from_json()
    {
        var original = SigabaFileV1.CreateDefault(publicKey);

        var json = original.Serialize();
        var actual = SigabaFileV1.Deserialize(json);

        actual.Should().NotBeNull();
    }

    // Version

    [Fact]
    public void Should_have_version_1()
    {
        var model = CreateModel();
        model.Version.Should().Be(1);
    }


    // FieldNamePredicate

    [Fact]
    public void Should_filter_field_names()
    {
        string[] input = ["aaaa", "bbbb", "ccccx"];
        string[] expected = ["ccccx"];
        var model = CreateModel(fieldRegex: @"x$");

        var result = input.Where(model.FieldNamePredicate).ToArray();

        result.Should().BeEquivalentTo(expected);
    }

}

