namespace CSVTransferApp.Core.Models;

public class ProcessingResult
{
    public string TableName { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public int RecordsProcessed { get; set; }
    public long FileSizeBytes { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string LogFileName { get; set; } = string.Empty;
    public List<string> Warnings { get; set; } = new();
}
