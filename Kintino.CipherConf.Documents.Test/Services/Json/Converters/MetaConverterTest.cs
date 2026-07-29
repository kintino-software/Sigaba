using System.Text.Json;

namespace Kintino.CipherConf.Documents.Services.Json.Converters;

public class MetaConverterTest
{
    // Mind indent and whitespace chars when inspecting serialized JSON strings
    private readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true,
        IndentSize = 4
    };

    // Serialization

    [Fact]
    public void Should_deserialize_json_to_meta_object()
    {
        var json = """
        {
            "field1": "value 1",
            "field2": 42,
            "parent1": {
                "field1": "value 11"
            }
        }
        """;
        var expected = new RawObject(
            fields: [new("field1", "\"value 1\""), new("field2", "42")],
            children: new()
            {
                ["parent1"] = new RawObject(
                    fields: [new("field1", "\"value 11\"")],
                    children: [])
            });

        var actual = JsonSerializer.Deserialize<RawObject>(json, options);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Should_deserialize_complex_json_to_meta_object()
    {
        var json = """
        {
            "a": "value a",
            "b": 2,
            "c": null,
            "d": [ "d0", "d1" ],
            "e": true,
            "c": {
                "a": "value a",
                "b": 2,
                "c": null,
                "d": [ "d0", "d1" ],
                "e": true
            }
        }
        """;
        var expected = new RawObject(
            fields: [
                new("a", "\"value a\""),
                new("b", "2"),
                new("c", "null"),
                new("d", "[ \"d0\", \"d1\" ]"),
                new("e", "true")
            ],
            children: new()
            {
                ["c"] = new RawObject(
                    fields: [
                        new("a", "\"value a\""),
                        new("b", "2"),
                        new("c", "null"),
                        new("d", "[ \"d0\", \"d1\" ]"),
                        new("e", "true")
                    ],
                    children: [])
            });

        var actual = JsonSerializer.Deserialize<RawObject>(json, options);

        actual.Should().BeEquivalentTo(expected);
    }

    // Deserialization

    [Fact]
    public void Should_serialize_meta_object_to_json()
    {
        var metaObject = new RawObject(
            fields: [new("field1", "\"value 1\""), new("field2", "42")],
            children: new()
            {
                ["parent1"] = new RawObject(
                    fields: [new("field1", "\"value 11\"")],
                    children: [])
            });
        var expectedJson = """
        {
            "field1": "value 1",
            "field2": 42,
            "parent1": {
                "field1": "value 11"
            }
        }
        """;

        var actualJson = JsonSerializer.Serialize(metaObject, options);

        actualJson.Should().Be(expectedJson);
    }

    // Round-trip test

    [Fact]
    public void Should_round_trip_meta_object_serialization_and_deserialization()
    {
        var originalMetaObject = new RawObject(
            fields: [new("field1", "\"value 1\""), new("field2", "42")],
            children: new()
            {
                ["parent1"] = new RawObject(
                    fields: [new("field1", "\"value 11\"")],
                    children: [])
            });
        var json = JsonSerializer.Serialize(originalMetaObject, options);

        var deserializedMetaObject = JsonSerializer.Deserialize<RawObject>(json, options);

        deserializedMetaObject.Should().BeEquivalentTo(originalMetaObject);
    }

    [Fact]
    public void Should_round_trip_complex_meta_object_serialization_and_deserialization()
    {
        var originalJson = """
        {
            "a": "value a",
            "b": 2,
            "c": null,
            "d": [
                "d0",
                "d1"
            ],
            "e": true,
            "c": {
                "a": "value a",
                "b": 2,
                "c": null,
                "d": [
                    "d0",
                    "d1"
                ],
                "e": true
            }
        }
        """;

        var deserialized = JsonSerializer.Deserialize<RawObject>(originalJson, options);
        var serializedJson = JsonSerializer.Serialize(deserialized, options);

        serializedJson.Should().Be(originalJson);
    }
}

