using System.Data;
using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Core.Interfaces;

public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string encryptedText);
}

public interface ICredentialManager
{
    string GetCredential(string key);
    void SaveCredential(string key, string value);
}

public interface IDatabaseConnectionManager
{
    Task<IDbConnection> GetConnectionAsync(string connectionName);
    void ReleaseConnection(string connectionName);
}

public interface IFileHeaderService
{
    Task<Dictionary<string, string>> GetHeaderMappingsAsync(string tableName);
    Task SaveHeaderMappingsAsync(string tableName, Dictionary<string, string> mappings);
}

public interface IJobManagerService
{
    Task<ProcessingResult> SubmitJobAsync(TransferJob job);
    Task<List<ProcessingResult>> GetJobResultsAsync();
    Task<JobStatus> GetJobStatusAsync(string jobId);
}