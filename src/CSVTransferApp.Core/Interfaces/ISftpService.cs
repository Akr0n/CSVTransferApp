namespace CSVTransferApp.Core.Interfaces;

public interface ISftpService
{
    Task UploadFileAsync(string connectionName, Stream fileStream, string fileName);
    Task<bool> TestConnectionAsync(string connectionName);
    Task<bool> FileExistsAsync(string connectionName, string fileName);
    Task DeleteFileAsync(string connectionName, string fileName);
    void Dispose();
}
