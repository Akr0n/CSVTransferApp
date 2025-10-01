using CSVTransferApp.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CSVTransferApp.Infrastructure.Health;

public class FileHealthCheckService : IHealthCheckService, IDisposable
{
    private readonly ILogger<FileHealthCheckService> _logger;
    private readonly IConfiguration _configuration;
    private readonly CancellationTokenSource _cts = new();
    private Task? _worker;
    private readonly string _healthFilePath;
    private readonly TimeSpan _interval;

    public FileHealthCheckService(ILogger<FileHealthCheckService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        var logsDir = _configuration["Logging:Path"] ?? "logs";
        if (!Directory.Exists(logsDir)) Directory.CreateDirectory(logsDir);

        _healthFilePath = Path.Combine(logsDir, "health.txt");

        if (!int.TryParse(_configuration["HealthCheck:IntervalSeconds"], out var seconds))
            seconds = 30;

        _interval = TimeSpan.FromSeconds(seconds);
    }

    public void Start()
    {
        if (_worker != null) return; // already started

        _worker = Task.Run(async () =>
        {
            _logger.LogInformation("HealthCheck started, writing heartbeat to {HealthFile}", _healthFilePath);
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    var line = $"{{\"timestamp\":\"{DateTime.UtcNow:o}\",\"status\":\"ok\"}}{Environment.NewLine}";
                    await File.AppendAllTextAsync(_healthFilePath, line, _cts.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to write health heartbeat");
                }

                try { await Task.Delay(_interval, _cts.Token); } catch (OperationCanceledException) { break; }
            }
            _logger.LogInformation("HealthCheck stopped");
        });
    }

    public async Task StopAsync()
    {
        try
        {
            _cts.Cancel();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cancel health worker");
        }

        if (_worker != null)
        {
            try
            {
                await _worker.ConfigureAwait(false);
            }
            catch (OperationCanceledException) { /* expected on shutdown */ }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Health worker terminated with error");
            }
            finally
            {
                _worker = null;
            }
        }
    }

    // Dispose pattern
    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            try { _cts.Cancel(); } catch { }
            try { _cts.Dispose(); } catch { }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
