namespace CSVTransferApp.Core.Models;

public class DatabaseConnectionConfig
{
    public string Name { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public int MaxPoolSize { get; set; } = 100;
    public int ConnectionTimeout { get; set; } = 30;
    public int CommandTimeout { get; set; } = 300;
}
