namespace CSVTransferApp.Core.Exceptions;

public class CsvProcessingException : Exception
{
    public string TableName { get; }

    public CsvProcessingException(string tableName, string message) 
        : base($"CSV processing for table '{tableName}': {message}")
    {
        TableName = tableName;
    }

    public CsvProcessingException(string tableName, string message, Exception innerException) 
        : base($"CSV processing for table '{tableName}': {message}", innerException)
    {
        TableName = tableName;
    }
}
