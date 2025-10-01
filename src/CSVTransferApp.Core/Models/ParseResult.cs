namespace CSVTransferApp.Core.Models;

public class ParseResult
{
    public Dictionary<string, string> Arguments { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public bool IsValid => Errors.Count == 0;
}