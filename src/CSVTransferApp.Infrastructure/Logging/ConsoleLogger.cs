using Microsoft.Extensions.Logging;

namespace CSVTransferApp.Infrastructure.Logging;

public class ConsoleLogger : ILogger
{
    private readonly string _categoryName;

    public ConsoleLogger(string categoryName)
    {
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, 
        Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var color = GetLogLevelColor(logLevel);
        var originalColor = Console.ForegroundColor;
        
        try
        {
            Console.ForegroundColor = color;
            var message = $"[{DateTime.Now:HH:mm:ss}] [{logLevel}] {formatter(state, exception)}";
            Console.WriteLine(message);
            
            if (exception != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception.ToString());
            }
        }
        finally
        {
            Console.ForegroundColor = originalColor;
        }
    }

    private static ConsoleColor GetLogLevelColor(LogLevel logLevel) => logLevel switch
    {
        LogLevel.Critical => ConsoleColor.Magenta,
        LogLevel.Error => ConsoleColor.Red,
        LogLevel.Warning => ConsoleColor.Yellow,
        LogLevel.Information => ConsoleColor.White,
        LogLevel.Debug => ConsoleColor.Gray,
        LogLevel.Trace => ConsoleColor.DarkGray,
        _ => ConsoleColor.White
    };
}
