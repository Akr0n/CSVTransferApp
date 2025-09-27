using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Interfaces;

namespace CSVTransferApp.Console.Commands;

public class TransferCommand : ICommand
{
    private readonly ICsvProcessingService _processingService;
    private readonly ILoggerService _logger;

    public TransferCommand(ICsvProcessingService processingService, ILoggerService logger)
    {
        _processingService = processingService;
        _logger = logger;
    }

    public async Task<int> ExecuteAsync(Dictionary<string, string> arguments)
    {
        if (!ValidateArguments(arguments, out var validationErrors))
        {
            foreach (var error in validationErrors)
            {
                _logger.LogError(error);
            }
            return 1;
        }

        var job = new TransferJob
        {
            TableName = arguments["table"],
            DatabaseConnection = arguments.GetValueOrDefault("db-connection", "Default"),
            SftpConnection = arguments.GetValueOrDefault("sftp-connection", "Default"),
            Query = arguments.GetValueOrDefault("query", $"SELECT * FROM {arguments["table"]}")
        };

        _logger.LogInformation("Starting transfer for table {TableName}", job.TableName);

        var result = await _processingService.ProcessJobAsync(job);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Transfer completed successfully for table {TableName}. Records: {RecordCount}, Size: {FileSize} bytes",
                result.TableName, result.RecordsProcessed, result.FileSizeBytes);
            return 0;
        }
        else
        {
            _logger.LogError("Transfer failed for table {TableName}: {Error}", result.TableName, result.ErrorMessage);
            return 1;
        }
    }

    private static bool ValidateArguments(Dictionary<string, string> arguments, out List<string> errors)
    {
        errors = new List<string>();

        if (!arguments.ContainsKey("table") || string.IsNullOrWhiteSpace(arguments["table"]))
        {
            errors.Add("--table parameter is required");
        }

        return errors.Count == 0;
    }
}
