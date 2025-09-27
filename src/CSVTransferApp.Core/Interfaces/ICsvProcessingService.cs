using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Core.Interfaces;

public interface ICsvProcessingService
{
    Task<ProcessingResult> ProcessJobAsync(TransferJob job);
    Task<List<ProcessingResult>> ProcessJobsAsync(IEnumerable<TransferJob> jobs);
}
