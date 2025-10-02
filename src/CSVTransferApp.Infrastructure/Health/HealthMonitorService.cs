using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Core.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace CSVTransferApp.Infrastructure.Health;

public class HealthMonitorService : IHealthCheckService
{
    private readonly ILogger<HealthMonitorService> _logger;
    private readonly IDatabaseService _databaseService;
    private readonly ISftpService _sftpService;
    private readonly ConcurrentDictionary<string, ServiceHealth> _healthStatus;

    public HealthMonitorService(
        ILogger<HealthMonitorService> logger,
        IDatabaseService databaseService,
        ISftpService sftpService)
    {
        _logger = logger;
        _databaseService = databaseService;
        _sftpService = sftpService;
        _healthStatus = new ConcurrentDictionary<string, ServiceHealth>();
    }

    public async Task<bool> CheckDatabaseConnectionAsync(string connectionName)
    {
        try
        {
            var isHealthy = await _databaseService.TestConnectionAsync(connectionName);
            _healthStatus.AddOrUpdate(
                $"db_{connectionName}",
                new ServiceHealth { IsHealthy = isHealthy, LastChecked = DateTime.UtcNow },
                (_, _) => new ServiceHealth { IsHealthy = isHealthy, LastChecked = DateTime.UtcNow }
            );
            return isHealthy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking database connection {ConnectionName}", connectionName);
            _healthStatus.AddOrUpdate(
                $"db_{connectionName}",
                new ServiceHealth { IsHealthy = false, LastChecked = DateTime.UtcNow, LastError = ex.Message },
                (_, _) => new ServiceHealth { IsHealthy = false, LastChecked = DateTime.UtcNow, LastError = ex.Message }
            );
            return false;
        }
    }

    public async Task<bool> CheckSftpConnectionAsync(string serverName)
    {
        try
        {
            var isHealthy = await _sftpService.TestConnectionAsync(serverName);
            _healthStatus.AddOrUpdate(
                $"sftp_{serverName}",
                new ServiceHealth { IsHealthy = isHealthy, LastChecked = DateTime.UtcNow },
                (_, _) => new ServiceHealth { IsHealthy = isHealthy, LastChecked = DateTime.UtcNow }
            );
            return isHealthy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking SFTP connection {ServerName}", serverName);
            _healthStatus.AddOrUpdate(
                $"sftp_{serverName}",
                new ServiceHealth { IsHealthy = false, LastChecked = DateTime.UtcNow, LastError = ex.Message },
                (_, _) => new ServiceHealth { IsHealthy = false, LastChecked = DateTime.UtcNow, LastError = ex.Message }
            );
            return false;
        }
    }

    public ServiceHealth GetServiceHealth(string serviceName)
    {
        return _healthStatus.TryGetValue(serviceName, out var health)
            ? health
            : new ServiceHealth { IsHealthy = false, LastChecked = DateTime.MinValue };
    }

    public IEnumerable<KeyValuePair<string, ServiceHealth>> GetAllServicesHealth()
    {
        return _healthStatus.ToArray();
    }

    public void Start()
    {
        _logger.LogInformation("Health monitoring service started");
    }

    public async Task StopAsync()
    {
        _logger.LogInformation("Health monitoring service stopping");
        _healthStatus.Clear();
        await Task.CompletedTask;
    }
}