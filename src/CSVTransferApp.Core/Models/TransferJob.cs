namespace CSVTransferApp.Core.Models;

/// <summary>
/// Rappresenta un job di trasferimento CSV da database a SFTP
/// </summary>
public class TransferJob
{
    /// <summary>
    /// Nome della tabella o identificativo del file CSV risultante
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Nome della connessione database configurata in appsettings.json
    /// </summary>
    public string DatabaseConnection { get; set; } = string.Empty;

    /// <summary>
    /// Nome della connessione SFTP configurata in appsettings.json
    /// </summary>
    public string SftpConnection { get; set; } = string.Empty;

    /// <summary>
    /// Query SQL da eseguire per estrarre i dati
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp di quando è stato richiesto il job
    /// </summary>
    public DateTime RequestTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ID univoco del job (generato automaticamente)
    /// </summary>
    public string JobId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Priorità del job (1 = alta, 5 = bassa)
    /// </summary>
    public int Priority { get; set; } = 3;

    /// <summary>
    /// Indica se applicare header override personalizzati
    /// </summary>
    public bool UseHeaderOverride { get; set; } = true;

    /// <summary>
    /// Nome utente che ha richiesto il job (per audit)
    /// </summary>
    public string RequestedBy { get; set; } = Environment.UserName;

    /// <summary>
    /// Stato del job
    /// </summary>
    public TransferJobStatus Status { get; set; } = TransferJobStatus.Pending;
}
