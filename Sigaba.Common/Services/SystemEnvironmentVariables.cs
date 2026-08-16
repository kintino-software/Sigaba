namespace Sigaba.Services;

internal class SystemEnvironmentVariables : IEnvironmentVariables
{
  string? IEnvironmentVariables.GetEnvironmentVariable(string variableName)
  {
    return Environment.GetEnvironmentVariable(variableName);
  }

  void IEnvironmentVariables.SetEnvironmentVariable(string variableName, string? value)
  {
    Environment.SetEnvironmentVariable(variableName, value);
  }
}
