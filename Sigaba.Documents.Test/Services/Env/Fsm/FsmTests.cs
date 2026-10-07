namespace Sigaba.Documents.Services.Env.Fsm;

public class FsmTests
{
    private IFsm CreateFsm()
    {
        return new Fsm();
    }

    [Fact]
    public void Should_process_simple_content()
    {
        var fsm = CreateFsm();
        // idx                   1          2          3      
        // idx:        01234567890 123456789012 345678901234 5
        var content = "key1=value1\nkey2=value2\nkey3=value3\n";

        var result = fsm.Process(content);

        result.Should().BeEquivalentTo([
            new EnvToken(){Key = "key1", Value = "value1", RawValueStartIndex = 5, RawValueEndIndex = 10 },
            new EnvToken(){Key = "key2", Value = "value2", RawValueStartIndex = 17, RawValueEndIndex = 22 },
            new EnvToken(){Key = "key3", Value = "value3", RawValueStartIndex = 29, RawValueEndIndex = 34 },
            ]);
    }

    [Fact]
    public void Should_process_value_continuations()
    {
        var fsm = CreateFsm();
        // idx                   1          2          3           4  
        // idx:        01234567890 123456789012 3456789 0123456 789012345678
        var content = "key1=value1\nkey2=multi1\\multi2\\multi3\nkey3=value3";

        var result = fsm.Process(content);

        result.Should().BeEquivalentTo([
            new EnvToken(){Key = "key1", RawValueStartIndex = 5, RawValueEndIndex = 10 },
            new EnvToken(){Key = "key2", RawValueStartIndex = 17, RawValueEndIndex = 37 },
            new EnvToken(){Key = "key3", RawValueStartIndex = 43, RawValueEndIndex = 48 },
            ]);
    }
}

