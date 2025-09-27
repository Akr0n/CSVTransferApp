namespace CSVTransferApp.Core.Exceptions;

public class SftpConnectionException : Exception
{
    public string ConnectionName { get; }

    public SftpConnectionException(string connectionName, string message) 
        : base($"SFTP connection '{connectionName}': {message}")
    {
        ConnectionName = connectionName;
    }

    public SftpConnectionException(string connectionName, string message, Exception innerException) 
        : base($"SFTP connection '{connectionName}': {message}", innerException)
    {
        ConnectionName = connectionName;
    }
}
