namespace CSVTransferApp.Core.Interfaces;

/// <summary>
/// Servizio per la gestione dei mapping degli header CSV
/// </summary>
public interface IFileHeaderService
{
    /// <summary>
    /// Recupera i mapping degli header per una specifica tabella
    /// </summary>
    Task<Dictionary<string, string>> GetHeaderMappingsAsync(string tableName);

    /// <summary>
    /// Salva i mapping degli header per una specifica tabella
    /// </summary>
    Task SaveHeaderMappingsAsync(string tableName, Dictionary<string, string> mappings);
}