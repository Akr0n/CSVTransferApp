using System.Text.Json;
using CSVTransferApp.Core.Exceptions;
using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Interfaces;

namespace CSVTransferApp.Services;
public class CsvProcessingService : ICsvProcessingService
{
    private readonly IDatabaseService _databaseService;
    private readonly ISftpService _sftpService;
    private readonly ILogger<CsvProcessingService> _logger;
    private readonly IConfiguration _configuration;
    private readonly SemaphoreSlim _connectionSemaphore;
    private readonly SemaphoreSlim _fileSemaphore;

    public CsvProcessingService(
        IDatabaseService databaseService,
        ISftpService sftpService,
        ILogger<CsvProcessingService> logger,
        IConfiguration configuration)
    {
        _databaseService = databaseService;
        _sftpService = sftpService;
        _logger = logger;
        _configuration = configuration;
        
        var maxConnections = _configuration.GetValue<int>("Processing:MaxConcurrentConnections");
        var maxFiles = _configuration.GetValue<int>("Processing:MaxConcurrentFiles");
        
        _connectionSemaphore = new SemaphoreSlim(maxConnections);
        _fileSemaphore = new SemaphoreSlim(maxFiles);
    }

    public async Task<List<ProcessingResult>> ProcessJobsAsync(IEnumerable<TransferJob> jobs)
    {
        var tasks = jobs.Select(ProcessJobAsync);
        var results = await Task.WhenAll(tasks);
        return results.ToList();
    }

    public async Task<ProcessingResult> ProcessJobAsync(TransferJob job)
    {
        await _connectionSemaphore.WaitAsync();
        await _fileSemaphore.WaitAsync();
        
        try
        {
            using var logScope = _logger.BeginScope("Job-{TableName}", job.TableName);
            
            _logger.LogInformation("Processing job for table {TableName}", job.TableName);
            var startTime = DateTime.UtcNow;
            
            // Esegui query e genera CSV
            var data = await _databaseService.ExecuteQueryAsync(job.DatabaseConnection, job.Query);
            var csvFileName = $"{job.TableName}.csv";
            var logFileName = $"{job.TableName}.log";
            
            // Controlla override headers
            var headers = await GetHeadersAsync(job.TableName, data);
            
            // Genera CSV e gestisci la risorsa
            await using var csvStream = GenerateCsvStream(data, headers);
            var fileSizeBytes = csvStream.Length;
            
            // Upload SFTP con retry in caso di errore
            try
            {
                await _sftpService.UploadFileAsync(job.SftpConnection, csvStream, csvFileName);
            }
            catch (SftpConnectionException ex)
            {
                _logger.LogWarning(ex, "SFTP upload failed for {TableName}, retrying...", job.TableName);
                // Riposiziona lo stream all'inizio per il retry
                csvStream.Position = 0;
                await _sftpService.UploadFileAsync(job.SftpConnection, csvStream, csvFileName);
            }
            
            // Genera log specifico
            await GenerateJobLogAsync(job, logFileName, headers, data.Rows.Count);
            
            _logger.LogInformation("Job completed for table {TableName}", job.TableName);
            var endTime = DateTime.UtcNow;
            
            return new ProcessingResult
            {
                IsSuccess = true,
                TableName = job.TableName,
                RecordsProcessed = data.Rows.Count,
                LogFileName = logFileName,
                FileSizeBytes = fileSizeBytes,
                StartTime = startTime,
                EndTime = endTime,
                ProcessingTime = endTime - startTime
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing job for table {TableName}", job.TableName);
            return new ProcessingResult
            {
                IsSuccess = false,
                TableName = job.TableName,
                ErrorMessage = ex.Message
            };
        }
        finally
        {
            _fileSemaphore.Release();
            _connectionSemaphore.Release();
        }
    }

    private async Task<string[]> GetHeadersAsync(string tableName, DataTable data)
    {
        var overridePath = _configuration.GetValue<string>("Processing:HeaderOverridePath") ?? "config/header-overrides";
        var overrideFile = Path.Combine(overridePath, $"{tableName}.json");
        
        if (File.Exists(overrideFile))
        {
            _logger.LogInformation("Using header override for table {TableName}", tableName);
            var json = await File.ReadAllTextAsync(overrideFile);
            var overrideData = JsonSerializer.Deserialize<HeaderOverride>(json);
            
            return data.Columns.Cast<DataColumn>()
                .Select(col => overrideData?.ColumnMappings?.GetValueOrDefault(col.ColumnName, col.ColumnName) ?? col.ColumnName)
                .ToArray();
        }
        
        return data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
    }

    private MemoryStream GenerateCsvStream(DataTable data, string[] headers)
    {
        var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
        
        // Scrivi headers
        writer.WriteLine(string.Join(",", headers.Select(EscapeCsvValue)));
        
        // Scrivi dati
        foreach (DataRow row in data.Rows)
        {
            var values = row.ItemArray.Select(field => EscapeCsvValue(field?.ToString() ?? ""));
            writer.WriteLine(string.Join(",", values));
        }
        
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
    
    private string EscapeCsvValue(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
        
        return value;
    }

    private async Task GenerateJobLogAsync(TransferJob job, string logFileName, string[] headers, int rowCount)
    {
        var logDir = _configuration.GetValue<string>("Processing:LogPath") ?? "logs/transfers";
        Directory.CreateDirectory(logDir);
        
        var logPath = Path.Combine(logDir, logFileName);
        var logContent = $"Transfer completed at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss UTC}\n" +
                        $"Table: {job.TableName}\n" +
                        $"Headers: {string.Join(", ", headers)}\n" +
                        $"Records processed: {rowCount}\n";
        
        await File.WriteAllTextAsync(logPath, logContent);
        _logger.LogInformation("Job log generated at {LogPath}", logPath);
    }
}
