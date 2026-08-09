using Sigaba.Services;

namespace Sigaba;

public class FakeEnvironmentVariables : IEnvironmentVariables
{
    private readonly Dictionary<string, string> variables = [];

    public string GetEnvironmentVariable(string variableName)
    {
        return variables[variableName];
    }

    public void SetEnvironmentVariable(string variableName, string value)
    {
        variables[variableName] = value;
    }
}
