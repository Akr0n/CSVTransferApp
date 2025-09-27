namespace CSVTransferApp.Core.Constants;

public static class LoggingConstants
{
    public const string ApplicationLoggerName = "CSVTransferApp";
    public const string DatabaseLoggerName = "CSVTransferApp.Database";
    public const string SftpLoggerName = "CSVTransferApp.Sftp";
    public const string ProcessingLoggerName = "CSVTransferApp.Processing";
    
    public const string LogFileNameFormat = "{0}-{1:yyyyMMdd}.log";
    public const string TransferLogFileNameFormat = "{0}-{1:yyyyMMdd-HHmmss}.log";
    
    public const string LogMessageFormat = "[{0:yyyy-MM-dd HH:mm:ss}] [{1}] {2}";
}
