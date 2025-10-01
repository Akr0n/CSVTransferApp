using System.Collections.Concurrent;
using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Interfaces;

namespace CSVTransferApp.Services;

public class JobManagerService : IJobManagerService
{
    private readonly ICsvProcessingService _processingService;
    private readonly ILoggerService _logger;
    private readonly SemaphoreSlim _concurrentJobsSemaphore;
    private readonly ConcurrentDictionary<string, ProcessingResult> _activeJobs;

    public JobManagerService(ICsvProcessingService processingService, ILoggerService logger, 
        IConfigurationService configurationService)
    {
        _processingService = processingService;
        _logger = logger;
        
        var maxConcurrentJobs = configurationService.GetValue("Processing:MaxConcurrentJobs", 5);
        _concurrentJobsSemaphore = new SemaphoreSlim(maxConcurrentJobs);
        _activeJobs = new ConcurrentDictionary<string, ProcessingResult>();
    }

    private readonly ConcurrentQueue<TransferJob> _jobQueue = new();
    
    public Task QueueJobAsync(TransferJob job)
    {
        _jobQueue.Enqueue(job);
        _logger.LogInformation("Job queued for table {TableName}", job.TableName);
        return Task.CompletedTask;
    }

    public async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_jobQueue.TryDequeue(out var job))
            {
                await SubmitJobAsync(job);
            }
            else
            {
                await Task.Delay(1000, cancellationToken); // Wait before checking queue again
            }
        }
    }

    public async Task<ProcessingResult> SubmitJobAsync(TransferJob job)
    {
        await _concurrentJobsSemaphore.WaitAsync();
        
        try
        {
            var jobId = $"{job.TableName}_{DateTime.UtcNow:yyyyMMddHHmmss}";
            _logger.LogInformation("Starting job {JobId} for table {TableName}", jobId, job.TableName);
            
            var result = await _processingService.ProcessJobAsync(job);
            result.LogFileName = $"{job.TableName}_{DateTime.UtcNow:yyyyMMdd-HHmmss}.log";
            
            _activeJobs.TryAdd(jobId, result);
            
            _logger.LogInformation("Completed job {JobId} for table {TableName} - Success: {Success}", 
                jobId, job.TableName, result.IsSuccess);
                
            return result;
        }
        finally
        {
            _concurrentJobsSemaphore.Release();
        }
    }

    public async Task<List<ProcessingResult>> SubmitBatchJobsAsync(IEnumerable<TransferJob> jobs)
    {
        var tasks = jobs.Select(SubmitJobAsync);
        var results = await Task.WhenAll(tasks);
        
        _logger.LogInformation("Completed batch processing of {JobCount} jobs. Success: {SuccessCount}, Failed: {FailedCount}",
            results.Length, results.Count(r => r.IsSuccess), results.Count(r => !r.IsSuccess));
            
        return results.ToList();
    }

    public IEnumerable<ProcessingResult> GetActiveJobs()
    {
        return _activeJobs.Values;
    }

    public void ClearCompletedJobs()
    {
        var completedJobs = _activeJobs.Where(kvp => kvp.Value.EndTime != default).ToList();
        foreach (var job in completedJobs)
        {
            _activeJobs.TryRemove(job.Key, out _);
        }
        
        _logger.LogInformation("Cleared {Count} completed jobs from memory", completedJobs.Count);
    }
}
