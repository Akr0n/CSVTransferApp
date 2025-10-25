using System.Text.Json;
using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Interfaces;

namespace CSVTransferApp.Console.Commands;

public class BatchCommand : ICommand
{
    private readonly ICsvProcessingService _processingService;
    private readonly ILoggerService _logger;

    public BatchCommand(ICsvProcessingService processingService, ILoggerService logger)
    {
        _processingService = processingService;
        _logger = logger;
    }

    public async Task<int> ExecuteAsync(Dictionary<string, string> arguments)
    {
        if (!arguments.ContainsKey("file"))
        {
            _logger.LogError("--file parameter is required for batch command", Array.Empty<object>());
            return 1;
        }

        var batchFile = arguments["file"];
        if (!File.Exists(batchFile))
        {
            _logger.LogError("Batch file not found: {File}", batchFile);
            return 1;
        }

        try
        {
            var json = await File.ReadAllTextAsync(batchFile);
            var jobs = JsonSerializer.Deserialize<List<TransferJob>>(json);

            if (jobs == null || !jobs.Any())
            {
                _logger.LogError("No valid jobs found in batch file", Array.Empty<object>());
                return 1;
            }

            _logger.LogInformation("Starting batch processing of {JobCount} jobs", jobs.Count);

            var results = await _processingService.ProcessJobsAsync(jobs);
            
            var successCount = results.Count(r => r.IsSuccess);
            var failureCount = results.Count(r => !r.IsSuccess);

            _logger.LogInformation("Batch processing completed. Success: {SuccessCount}, Failed: {FailureCount}",
                successCount, failureCount);

            // Log details of failed jobs
            foreach (var failedResult in results.Where(r => !r.IsSuccess))
            {
                _logger.LogError("Failed job - Table: {TableName}, Error: {Error}",
                    failedResult.TableName, failedResult.ErrorMessage ?? string.Empty);
            }

            return failureCount == 0 ? 0 : 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing batch file: {File}", batchFile);
            return 1;
        }
    }
}
