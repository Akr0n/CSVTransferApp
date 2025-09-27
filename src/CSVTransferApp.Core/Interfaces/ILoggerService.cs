namespace CSVTransferApp.Core.Interfaces;

public interface ILoggerService
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(Exception exception, string message, params object[] args);
    void LogError(string message, params object[] args);
    Task LogToFileAsync(string fileName, string message);
}
