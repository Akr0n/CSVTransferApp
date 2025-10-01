using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Console.Parsers;

public interface ICommandLineParser
{
    ParseResult ParseArguments(string[] args);
}