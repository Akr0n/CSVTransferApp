using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Console.Commands;
using CSVTransferApp.Console.Parsers;

namespace CSVTransferApp.Console;

public class Application
{
    private readonly ICsvProcessingService _processingService;
    private readonly ILoggerService _logger;
    private readonly ICommandLineParser _parser;
    private readonly Dictionary<string, ICommand> _commands;

    public Application(ICsvProcessingService processingService, ILoggerService logger, ICommandLineParser parser)
    {
        _processingService = processingService;
        _logger = logger;
        _parser = parser;
        
        _commands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase)
        {
            { "transfer", new TransferCommand(_processingService, _logger) },
            { "batch", new BatchCommand(_processingService, _logger) },
            { "test", new TestConnectionCommand(_logger) },
            { "help", new HelpCommand() }
        };
    }

    public async Task<int> RunAsync(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                await _commands["help"].ExecuteAsync(new Dictionary<string, string>());
                return 1;
            }

            var commandName = args[0].ToLower();
            
            if (!_commands.TryGetValue(commandName, out var command))
            {
                _logger.LogError("Unknown command: {Command}", commandName);
                await _commands["help"].ExecuteAsync(new Dictionary<string, string>());
                return 1;
            }

            var parsedArgs = _parser.ParseArguments(args.Skip(1).ToArray());
            
            if (!parsedArgs.IsValid)
            {
                _logger.LogError("Invalid arguments: {Errors}", string.Join(", ", parsedArgs.Errors));
                return 1;
            }

            return await command.ExecuteAsync(parsedArgs.Arguments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Application error occurred");
            return 1;
        }
    }
}
