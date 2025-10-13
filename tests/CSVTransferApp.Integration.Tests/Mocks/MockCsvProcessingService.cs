using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Core.Models;
using Microsoft.Extensions.Logging;

namespace CSVTransferApp.Integration.Tests.Mocks;

public class MockCsvProcessingService : ICsvProcessingService
{
    private readonly ILogger<MockCsvProcessingService> _logger;

    public MockCsvProcessingService(ILogger<MockCsvProcessingService> logger)
    {
        _logger = logger;
    }

    public Task<ProcessingResult> ProcessJobAsync(TransferJob job)
    {
        _logger.LogInformation("Processing job {JobId} for table {TableName}", job.JobId, job.TableName);
        job.Status = TransferJobStatus.Completed;
        
        var result = new ProcessingResult
        {
            IsSuccess = true,
            RecordsProcessed = 100 // Mock value
        };
        
        return Task.FromResult(result);
    }

    public Task<List<ProcessingResult>> ProcessJobsAsync(IEnumerable<TransferJob> jobs)
    {
        var results = new List<ProcessingResult>();
        
        foreach (var job in jobs)
        {
            job.Status = TransferJobStatus.Completed;
            results.Add(new ProcessingResult
            {
                IsSuccess = true,
                RecordsProcessed = 100 // Mock value
            });
        }
        return Task.FromResult(results);
    }
}