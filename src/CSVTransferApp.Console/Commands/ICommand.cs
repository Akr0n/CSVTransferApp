namespace CSVTransferApp.Console.Commands;

public interface ICommand
{
    Task<int> ExecuteAsync(Dictionary<string, string> arguments);
}
