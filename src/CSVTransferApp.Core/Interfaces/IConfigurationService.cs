using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Core.Interfaces;

public interface IConfigurationService
{
    DatabaseConnectionConfig GetDatabaseConnection(string name);
    SftpConnectionConfig GetSftpConnection(string name);
    T GetValue<T>(string key, T defaultValue = default!);
    IEnumerable<DatabaseConnectionConfig> GetAllDatabaseConnections();
    IEnumerable<SftpConnectionConfig> GetAllSftpConnections();
}
