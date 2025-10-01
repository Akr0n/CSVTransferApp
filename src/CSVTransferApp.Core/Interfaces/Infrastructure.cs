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

public interface IDataAdapterFactory
{
    /// <summary>
    /// Crea un data adapter per il provider specificato
    /// </summary>
    /// <param name="providerName">Nome del provider database</param>
    /// <param name="command">Comando da associare al data adapter</param>
    /// <returns>Data adapter specifico per il provider</returns>
    IDbDataAdapter CreateDataAdapter(string providerName, IDbCommand command);

    /// <summary>
    /// Verifica se un provider è supportato
    /// </summary>
    /// <param name="providerName">Nome del provider da verificare</param>
    /// <returns>true se il provider è supportato, false altrimenti</returns>
    bool IsProviderSupported(string providerName);
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