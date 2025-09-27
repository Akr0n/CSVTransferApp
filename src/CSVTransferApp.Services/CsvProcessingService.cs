// Services/CsvProcessingService.cs
public class CsvProcessingService
{
    private readonly DatabaseService _databaseService;
    private readonly SftpService _sftpService;
    private readonly ILogger<CsvProcessingService> _logger;
    private readonly IConfiguration _configuration;
    private readonly SemaphoreSlim _connectionSemaphore;
    private readonly SemaphoreSlim _fileSemaphore;

    public CsvProcessingService(
        DatabaseService databaseService,
        SftpService sftpService,
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

    public async Task ProcessJobsAsync(IEnumerable<TransferJob> jobs)
    {
        var tasks = jobs.Select(ProcessJobAsync);
        await Task.WhenAll(tasks);
    }

    private async Task ProcessJobAsync(TransferJob job)
    {
        await _connectionSemaphore.WaitAsync();
        await _fileSemaphore.WaitAsync();
        
        try
        {
            using var logScope = _logger.BeginScope("Job-{TableName}", job.TableName);
            
            _logger.LogInformation("Processing job for table {TableName}", job.TableName);
            
            // Esegui query e genera CSV
            var data = await _databaseService.ExecuteQueryAsync(job.DatabaseConnection, job.Query);
            var csvFileName = $"{job.TableName}.csv";
            var logFileName = $"{job.TableName}.log";
            
            // Controlla override headers
            var headers = await GetHeadersAsync(job.TableName, data);
            
            // Genera CSV
            using var csvStream = GenerateCsvStream(data, headers);
            
            // Upload SFTP
            await _sftpService.UploadFileAsync(job.SftpConnection, csvStream, csvFileName);
            
            // Genera log specifico
            await GenerateJobLogAsync(job, logFileName, headers, data.Rows.Count);
            
            _logger.LogInformation("Job completed for table {TableName}", job.TableName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing job for table {TableName}", job.TableName);
            throw;
        }
        finally
        {
            _fileSemaphore.Release();
            _connectionSemaphore.Release();
        }
    }

    private async Task<string[]> GetHeadersAsync(string tableName, DataTable data)
    {
        var overridePath = _configuration.GetValue<string>("Processing:HeaderOverridePath");
        var overrideFile = Path.Combine(overridePath, $"{tableName}.json");
        
        if (File.Exists(overrideFile))
        {
            _logger.LogInformation("Using header override for table {TableName}", tableName);
            var json = await File.ReadAllTextAsync(overrideFile);
            var overrideData = JsonSerializer.Deserialize<HeaderOverride>(json);
            
            return data.Columns.Cast<DataColumn>()
                .Select(col => overrideData.ColumnMappings.GetValueOrDefault(col.ColumnName, col.ColumnName))
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
}
