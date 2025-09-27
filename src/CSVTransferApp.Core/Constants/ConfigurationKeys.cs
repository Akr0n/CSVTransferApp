namespace CSVTransferApp.Core.Constants;

public static class ConfigurationKeys
{
    public const string DatabaseConnections = "DatabaseConnections";
    public const string SftpConnections = "SftpConnections";
    public const string Processing = "Processing";
    public const string Logging = "Logging";
    
    public static class ProcessingKeys
    {
        public const string MaxConcurrentConnections = "Processing:MaxConcurrentConnections";
        public const string MaxConcurrentFiles = "Processing:MaxConcurrentFiles";
        public const string HeaderOverridePath = "Processing:HeaderOverridePath";
        public const string LogPath = "Processing:LogPath";
        public const string TempPath = "Processing:TempPath";
    }
    
    public static class LoggingKeys
    {
        public const string LogLevel = "Logging:LogLevel:Default";
        public const string LogToFile = "Logging:LogToFile";
        public const string LogPath = "Logging:LogPath";
        public const string MaxLogFileSizeMB = "Logging:MaxLogFileSizeMB";
    }
}
