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
            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("--"))
                {
                    result.Errors.Add($"Invalid argument format: {args[i]}");
                    continue;
                }

                var key = args[i].Substring(2);
                
                if (i + 1 >= args.Length || args[i + 1].StartsWith("--"))
                {
                    result.Errors.Add($"Missing value for argument: {key}");
                    continue;
                }

                arguments[key] = args[i + 1];
                i++;
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