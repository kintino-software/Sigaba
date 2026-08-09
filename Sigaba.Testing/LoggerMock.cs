using Microsoft.Extensions.Logging;

namespace Sigaba;

public class LoggerMock<T> : ILogger<T>
{
    private record Entry(LogLevel Level, string Message);
    private readonly List<Entry> entries = [];

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        entries.Add(new Entry(logLevel, formatter(state, exception)));
    }

    public void VerifyLog(LogLevel logLevel, string message)
    {
        if (entries.Contains(new Entry(logLevel, message))) return;
        throw new Exception("Expected log entry not found: " + logLevel + " - " + message);
    }
}
