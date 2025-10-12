using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Console.Parsers;

public class CommandLineParser : ICommandLineParser
{
    public ParseResult ParseArguments(string[] args)
    {
        var result = new ParseResult();
        var arguments = new Dictionary<string, string>();

        try
        {
            string? pendingKey = null;

            for (int i = 0; i < args.Length; i++)
            {
                var current = args[i];

                if (current.StartsWith("--"))
                {
                    if (pendingKey != null)
                    {
                        result.Errors.Add($"Missing value for argument: {pendingKey}");
                    }

                    pendingKey = current.Substring(2);
                }
                else
                {
                    if (pendingKey != null)
                    {
                        arguments[pendingKey] = current;
                        pendingKey = null;
                    }
                    else
                    {
                        result.Errors.Add($"Invalid argument format: {current}");
                    }
                }
            }

            if (pendingKey != null)
            {
                result.Errors.Add($"Missing value for argument: {pendingKey}");
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Error parsing arguments: {ex.Message}");
        }

        result.Arguments = arguments;
        // IsValid è una proprietà calcolata automaticamente

        return result;
    }
}