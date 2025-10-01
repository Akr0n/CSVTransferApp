namespace CSVTransferApp.Core.Models;

/// <summary>
/// Stati possibili di un job di trasferimento CSV
/// </summary>
public enum TransferJobStatus
{
    /// <summary>
    /// Job in attesa di esecuzione
    /// </summary>
    Pending,

    /// <summary>
    /// Job in corso di esecuzione
    /// </summary>
    Running,

    /// <summary>
    /// Job completato con successo
    /// </summary>
    Completed,

    /// <summary>
    /// Job fallito
    /// </summary>
    Failed,

    /// <summary>
    /// Job cancellato
    /// </summary>
    Cancelled
}