namespace CSVTransferApp.Core.Interfaces;

public interface IHealthCheckService
{
    /// <summary>
    /// Start the health check background loop.
    /// </summary>
    void Start();

    /// <summary>
    /// Stop the health check background loop and wait for graceful shutdown.
    /// </summary>
    Task StopAsync();
}
