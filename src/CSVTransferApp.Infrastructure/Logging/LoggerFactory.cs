using Microsoft.Extensions.Logging;

namespace CSVTransferApp.Infrastructure.Logging;

public class CustomLoggerFactory : ILoggerFactory
{
    private readonly Dictionary<string, ILogger> _loggers = new();
    private readonly string _logPath;

    public CustomLoggerFactory(string logPath)
    {
        _logPath = logPath;
    }

    public void AddProvider(ILoggerProvider provider)
    {
        // Implementation for adding providers if needed
    }

    public ILogger CreateLogger(string categoryName)
    {
        if (!_loggers.TryGetValue(categoryName, out var logger))
        {
            var logFileName = Path.Combine(_logPath, $"{categoryName.Replace('.', '-')}.log");
            logger = new CompositeLogger(categoryName, logFileName);
            _loggers[categoryName] = logger;
        }
        
        return logger;
    }

    public void Dispose()
    {
        _loggers.Clear();
        GC.SuppressFinalize(this);
    }
}

public class CompositeLogger : ILogger
{
    private readonly FileLogger _fileLogger;
    private readonly ConsoleLogger _consoleLogger;

    public CompositeLogger(string categoryName, string logFileName)
    {
        _fileLogger = new FileLogger(categoryName, logFileName);
        _consoleLogger = new ConsoleLogger(categoryName);
    }

    public IDisposable? BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => _fileLogger.IsEnabled(logLevel) || _consoleLogger.IsEnabled(logLevel);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _fileLogger.Log(logLevel, eventId, state, exception, formatter);
        _consoleLogger.Log(logLevel, eventId, state, exception, formatter);
    }
}
