using Microsoft.Extensions.Logging;
using CSVTransferApp.Core.Constants;

namespace CSVTransferApp.Infrastructure.Logging;

public class FileLogger : ILogger
{
    private readonly string _categoryName;
    private readonly string _logPath;
    private readonly object _lock = new();

    public FileLogger(string categoryName, string logPath)
    {
        _categoryName = categoryName;
        _logPath = logPath;
        
        Directory.CreateDirectory(Path.GetDirectoryName(_logPath)!);
    }

    public IDisposable? BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, 
        Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = formatter(state, exception);
        var logEntry = string.Format(LoggingConstants.LogMessageFormat,
            DateTime.UtcNow, logLevel.ToString().ToUpper(), message);

        if (exception != null)
        {
            logEntry += Environment.NewLine + exception.ToString();
        }

        lock (_lock)
        {
            File.AppendAllText(_logPath, logEntry + Environment.NewLine);
        }
    }
}
