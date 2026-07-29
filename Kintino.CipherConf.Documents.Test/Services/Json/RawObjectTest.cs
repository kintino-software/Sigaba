namespace Kintino.CipherConf.Documents.Services.Json;

public class RawObjectTest
{
    // instantiation

    [Fact]
    public void Should_instantiate()
    {
        var metaObj = new RawObject([], []);

        metaObj.Fields.Should().BeEmpty();
        metaObj.Children.Should().BeEmpty();
    }

    [Fact]
    public void Should_throw_if_fields_are_null()
    {
        var action = () => new RawObject(null, []);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_throw_if_children_are_null()
    {
        var action = () => new RawObject([], null);
        action.Should().Throw<ArgumentNullException>();
    }

    // GetFieldPaths

    [Fact]
    public void Should_return_field_paths()
    {
        var metaObj = new RawObject(
            [new RawField("field1", "0"), new RawField("field2", "1")],
            new Dictionary<string, RawObject>
            {
                { "child1", new RawObject(
                    [new RawField("childField1", "2")],
                    new Dictionary<string, RawObject>()
                    {
                        { "grandChild1", new RawObject(
                            [new RawField("grandChildField1", "3")], [])
                        }
                    })
                }
            }
        );
        var paths = metaObj.GetFieldPaths();
        paths.Should().BeEquivalentTo(["field1", "field2", "child1.childField1", "child1.grandChild1.grandChildField1"]);
    }

    // GetFieldByPath

    [Fact]
    public void Should_return_field_by_path()
    {
        var metaObj = new RawObject(
            [new RawField("field1", "1"), new RawField("field2", "2")],
            new Dictionary<string, RawObject>
            {
                { "child1", new RawObject(
                    [new RawField("childField1", "11")],
                    new Dictionary<string, RawObject>()
                    {
                        { "grandChild1", new RawObject(
                            [new RawField("grandChildField1", "111")], [])
                        }
                    })
                }
            }
        );

        var field1 = metaObj.GetFieldByPath("field1");
        field1.Should().NotBeNull();
        field1.Key.Should().Be("field1");

        var field2 = metaObj.GetFieldByPath("field2");
        field2.Should().NotBeNull();
        field2.Key.Should().Be("field2");

        var childField1 = metaObj.GetFieldByPath("child1.childField1");
        childField1.Should().NotBeNull();
        childField1.Key.Should().Be("childField1");

        var grandChildField1 = metaObj.GetFieldByPath("child1.grandChild1.grandChildField1");
        grandChildField1.Should().NotBeNull();
        grandChildField1.Key.Should().Be("grandChildField1");
    }
}
