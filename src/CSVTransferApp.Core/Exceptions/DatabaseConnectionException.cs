namespace CSVTransferApp.Core.Exceptions;

public class DatabaseConnectionException : Exception
{
    public string ConnectionName { get; }

    public DatabaseConnectionException(string connectionName, string message) 
        : base($"Database connection '{connectionName}': {message}")
    {
        ConnectionName = connectionName;
    }

    public DatabaseConnectionException(string connectionName, string message, Exception innerException) 
        : base($"Database connection '{connectionName}': {message}", innerException)
    {
        ConnectionName = connectionName;
    }
}
