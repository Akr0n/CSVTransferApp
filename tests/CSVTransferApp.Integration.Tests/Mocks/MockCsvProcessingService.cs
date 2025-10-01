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

    public Task ProcessJobAsync(TransferJob job)
    {
        _logger.LogInformation("Processing job {JobId} for table {TableName}", job.JobId, job.TableName);
        job.Status = TransferJobStatus.Completed;
        return Task.CompletedTask;
    }

    public Task ProcessJobsAsync(IEnumerable<TransferJob> jobs)
    {
        foreach (var job in jobs)
        {
            job.Status = TransferJobStatus.Completed;
        }
        return Task.CompletedTask;
    }
}