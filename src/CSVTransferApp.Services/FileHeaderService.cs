using System.Text.Json;
using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Core.Constants;

namespace CSVTransferApp.Services;

public class FileHeaderService : IFileHeaderService
{
    private readonly ILoggerService _logger;
    private readonly string _overridePath;

    public FileHeaderService(IConfigurationService configurationService, ILoggerService logger)
    {
        _logger = logger;
        _overridePath = configurationService.GetValue(ConfigurationKeys.ProcessingKeys.HeaderOverridePath, "./config/header-overrides");
        
        // Ensure directory exists
        Directory.CreateDirectory(_overridePath);
    }

    public async Task<Dictionary<string, string>> GetHeaderMappingsAsync(string tableName)
    {
        var overrideFile = Path.Combine(_overridePath, $"{tableName}.json");
        
        if (!File.Exists(overrideFile))
        {
            _logger.LogInformation("No header mappings found for table {TableName}", tableName);
            return new Dictionary<string, string>();
        }

        try
        {
            var json = await File.ReadAllTextAsync(overrideFile);
            var headerOverride = JsonSerializer.Deserialize<HeaderOverride>(json);
            return headerOverride?.ColumnMappings ?? new Dictionary<string, string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading header mappings for table {TableName}", tableName);
            return new Dictionary<string, string>();
        }
    }

    public async Task SaveHeaderMappingsAsync(string tableName, Dictionary<string, string> mappings)
    {
        var overrideFile = Path.Combine(_overridePath, $"{tableName}.json");
        var headerOverride = new HeaderOverride
        {
            TableName = tableName,
            ColumnMappings = mappings
        };

        try
        {
            var json = JsonSerializer.Serialize(headerOverride, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(overrideFile, json);
            _logger.LogInformation("Saved header mappings for table {TableName}", tableName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving header mappings for table {TableName}", tableName);
            throw;
        }
    }

    public async Task<string[]> GetHeadersAsync(string tableName, string[] originalHeaders)
    {
        var overrideFile = Path.Combine(_overridePath, $"{tableName}.json");
        
        if (!File.Exists(overrideFile))
        {
            _logger.LogInformation("No header override found for table {TableName}, using original headers", tableName);
            return originalHeaders;
        }

        try
        {
            _logger.LogInformation("Loading header override for table {TableName}", tableName);
            var json = await File.ReadAllTextAsync(overrideFile);
            var overrideData = JsonSerializer.Deserialize<HeaderOverride>(json);
            
            if (overrideData?.ColumnMappings == null)
            {
                _logger.LogWarning("Invalid header override file for table {TableName}, using original headers", tableName);
                return originalHeaders;
            }

            return originalHeaders.Select(header => 
                overrideData.ColumnMappings.GetValueOrDefault(header, header)).ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading header override for table {TableName}, using original headers", tableName);
            return originalHeaders;
        }
    }

    public async Task SaveHeaderOverrideAsync(string tableName, Dictionary<string, string> mappings)
    {
        var overrideFile = Path.Combine(_overridePath, $"{tableName}.json");
        var headerOverride = new HeaderOverride { ColumnMappings = mappings };
        
        var json = JsonSerializer.Serialize(headerOverride, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(overrideFile, json);
        
        _logger.LogInformation("Saved header override for table {TableName}", tableName);
    }

    public bool HasOverride(string tableName)
    {
        var overrideFile = Path.Combine(_overridePath, $"{tableName}.json");
        return File.Exists(overrideFile);
    }
}
