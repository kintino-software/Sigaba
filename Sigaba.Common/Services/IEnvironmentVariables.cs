namespace Sigaba.Services;

public interface IEnvironmentVariables
{
    string? GetEnvironmentVariable(string variableName);
    void SetEnvironmentVariable(string variableName, string? value);
}

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
