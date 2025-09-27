using Microsoft.Extensions.Logging;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Core.Constants;

namespace CSVTransferApp.Services;

public class LoggingService : ILoggerService
{
    private readonly ILogger<LoggingService> _logger;
    private readonly string _logPath;

    public LoggingService(ILogger<LoggingService> logger, IConfigurationService configurationService)
    {
        _logger = logger;
        _logPath = configurationService.GetValue(ConfigurationKeys.ProcessingKeys.LogPath, "./logs");
        
        // Ensure log directory exists
        Directory.CreateDirectory(_logPath);
    }

    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        _logger.LogError(exception, message, args);
    }

    public void LogError(string message, params object[] args)
    {
        _logger.LogError(message, args);
    }

    public async Task LogToFileAsync(string fileName, string message)
    {
        var logFile = Path.Combine(_logPath, fileName);
        var logEntry = string.Format(LoggingConstants.LogMessageFormat, 
            DateTime.UtcNow, "INFO", message);
        
        await File.AppendAllTextAsync(logFile, logEntry + Environment.NewLine);
    }
}
