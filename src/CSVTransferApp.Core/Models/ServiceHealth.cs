namespace CSVTransferApp.Core.Models;

public class ServiceHealth
{
    public bool IsHealthy { get; set; }
    public DateTime LastChecked { get; set; }
    public string? LastError { get; set; }
}