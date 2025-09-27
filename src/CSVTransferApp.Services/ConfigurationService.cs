using Microsoft.Extensions.Configuration;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Extensions;

namespace CSVTransferApp.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;

    public ConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DatabaseConnectionConfig GetDatabaseConnection(string name)
    {
        return _configuration.GetDatabaseConnection(name);
    }

    public SftpConnectionConfig GetSftpConnection(string name)
    {
        return _configuration.GetSftpConnection(name);
    }

    public T GetValue<T>(string key, T defaultValue = default!)
    {
        return _configuration.GetValue<T>(key) ?? defaultValue;
    }

    public IEnumerable<DatabaseConnectionConfig> GetAllDatabaseConnections()
    {
        return _configuration.GetAllDatabaseConnections();
    }

    public IEnumerable<SftpConnectionConfig> GetAllSftpConnections()
    {
        return _configuration.GetAllSftpConnections();
    }
}
