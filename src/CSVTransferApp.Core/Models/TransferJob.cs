public class TransferJob
{
    public string TableName { get; set; }
    public string DatabaseConnection { get; set; }
    public string Query { get; set; }
    public string SftpConnection { get; set; }
    public DateTime RequestTime { get; set; } = DateTime.UtcNow;
}