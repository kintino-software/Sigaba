using Sigaba.Services;

namespace Sigaba;

public class FakeEnvironmentVariables : IEnvironmentVariables
{
  private readonly Dictionary<string, string> variables = [];

  public string GetEnvironmentVariable(string variableName)
  {
    if (variables.TryGetValue(variableName, out var value))
    {
      return value;
    }
    return null;
  }

  public void SetEnvironmentVariable(string variableName, string value)
  {
    variables[variableName] = value;
  }
}
