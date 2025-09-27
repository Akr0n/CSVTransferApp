namespace CSVTransferApp.Console.Parsers;

public class ArgumentValidator
{
    public static bool ValidateTableName(string tableName, out string error)
    {
        error = string.Empty;
        
        if (string.IsNullOrWhiteSpace(tableName))
        {
            error = "Table name cannot be empty";
            return false;
        }

        if (tableName.Length > 128)
        {
            error = "Table name cannot exceed 128 characters";
            return false;
        }

        // Basic validation for SQL injection prevention
        if (tableName.Contains("'") || tableName.Contains("\"") || tableName.Contains(";"))
        {
            error = "Table name contains invalid characters";
            return false;
        }

        return true;
    }

    public static bool ValidateConnectionName(string connectionName, out string error)
    {
        error = string.Empty;
        
        if (string.IsNullOrWhiteSpace(connectionName))
        {
            error = "Connection name cannot be empty";
            return false;
        }

        return true;
    }

    public static bool ValidateFilePath(string filePath, out string error)
    {
        error = string.Empty;
        
        if (string.IsNullOrWhiteSpace(filePath))
        {
            error = "File path cannot be empty";
            return false;
        }

        try
        {
            Path.GetFullPath(filePath);
        }
        catch
        {
            error = "Invalid file path format";
            return false;
        }

        return true;
    }
}

public class ParseResult
{
    public Dictionary<string, string> Arguments { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public bool IsValid => !Errors.Any();
}
