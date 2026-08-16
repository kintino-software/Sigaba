namespace Sigaba.Services;

public sealed class SystemEnvironmentVariablesTests : IDisposable
{
    private readonly string key = "TEMP_VARIABLE_ERASE_ME";
    private readonly IEnvironmentVariables service = new SystemEnvironmentVariables();

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(key, null);
    }

    // GetEnvironmentVariable

    [Fact]
    public void Should_get_environment_variable()
    {
        Environment.SetEnvironmentVariable(key, "value");
        var actual = service.GetEnvironmentVariable(key);
        actual.Should().Be("value");
    }

    [Fact]
    public void Should_return_null_when_getting_inexistent_environment_variable()
    {
        var actual = service.GetEnvironmentVariable("inexistent_variable_xxxxxxxxxxxxxxxxxxxxx");
        actual.Should().BeNull();
    }

    // SetEnvironmentVariable

    [Fact]
    public void Should_set_environment_variable()
    {
        service.SetEnvironmentVariable(key, "value");
        var actual = Environment.GetEnvironmentVariable(key);
        actual.Should().Be("value");
    }

}

