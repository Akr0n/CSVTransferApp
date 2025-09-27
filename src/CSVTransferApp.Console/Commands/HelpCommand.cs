namespace CSVTransferApp.Console.Commands;

public class HelpCommand : ICommand
{
    public Task<int> ExecuteAsync(Dictionary<string, string> arguments)
    {
        System.Console.WriteLine(@"
CSV Transfer Application

Usage: CSVTransferApp <command> [options]

Commands:
  transfer    Transfer a single table to CSV and upload via SFTP
  batch       Process multiple transfers from a batch file
  test        Test database and SFTP connections
  help        Show this help message

Transfer Command Options:
  --table <name>              Table name to export (required)
  --db-connection <name>      Database connection name (default: Default)
  --sftp-connection <name>    SFTP connection name (default: Default)
  --query <sql>              Custom SQL query (default: SELECT * FROM <table>)

Batch Command Options:
  --file <path>              Path to JSON batch file (required)

Examples:
  CSVTransferApp transfer --table employees --db-connection Oracle --sftp-connection MainServer
  CSVTransferApp batch --file batch-jobs.json
  CSVTransferApp test
");

        return Task.FromResult(0);
    }
}
