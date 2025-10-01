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
    private readonly IHealthCheckService _health;
    private readonly Dictionary<string, ICommand> _commands;

    public Application(ICsvProcessingService processingService, ILoggerService logger, ICommandLineParser parser, IHealthCheckService health)
    {
        _processingService = processingService;
        _logger = logger;
        _parser = parser;
        _health = health;

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
            // Start health loop so long-running service or on-demand runs write heartbeats
            try
            {
                _health?.Start();
            }
            catch (Exception startEx)
            {
                _logger.LogError(startEx, "Failed to start health check service");
            }

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

            var exit = await command.ExecuteAsync(parsedArgs.Arguments);

            // ensure health is stopped cleanly
            try
            {
                if (_health != null) await _health.StopAsync();
            }
            catch (Exception stopEx)
            {
                _logger.LogError(stopEx, "Failed to stop health check service");
            }

            return exit;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Application error occurred");
            try
            {
                if (_health != null) await _health.StopAsync();
            }
            catch (Exception stopEx)
            {
                _logger.LogError(stopEx, "Failed to stop health check service");
            }
            return 1;
        }
    }
}
