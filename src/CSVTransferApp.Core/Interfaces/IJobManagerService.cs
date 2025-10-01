using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Core.Interfaces;

/// <summary>
/// Servizio per la gestione dei job di trasferimento
/// </summary>
public interface IJobManagerService
{
    /// <summary>
    /// Aggiunge un nuovo job alla coda
    /// </summary>
    Task QueueJobAsync(TransferJob job);

    /// <summary>
    /// Elabora i job in coda
    /// </summary>
    Task ProcessQueueAsync(CancellationToken cancellationToken);
}