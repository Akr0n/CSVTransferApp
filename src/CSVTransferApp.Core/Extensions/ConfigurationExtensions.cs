using Microsoft.Extensions.Configuration;
using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Core.Extensions;

public static class ConfigurationExtensions
{
    public static DatabaseConnectionConfig GetDatabaseConnection(this IConfiguration configuration, string name)
    {
        var section = configuration.GetSection($"DatabaseConnections:{name}");
        var config = new DatabaseConnectionConfig { Name = name };
        section.Bind(config);
        return config;
    }

    public static SftpConnectionConfig GetSftpConnection(this IConfiguration configuration, string name)
    {
        var section = configuration.GetSection($"SftpConnections:{name}");
        var config = new SftpConnectionConfig { Name = name };
        section.Bind(config);
        return config;
    }

    public static IEnumerable<DatabaseConnectionConfig> GetAllDatabaseConnections(this IConfiguration configuration)
    {
        var section = configuration.GetSection("DatabaseConnections");
        foreach (var child in section.GetChildren())
        {
            var config = new DatabaseConnectionConfig { Name = child.Key };
            child.Bind(config);
            yield return config;
        }
    }

    public static IEnumerable<SftpConnectionConfig> GetAllSftpConnections(this IConfiguration configuration)
    {
        var section = configuration.GetSection("SftpConnections");
        foreach (var child in section.GetChildren())
        {
            var config = new SftpConnectionConfig { Name = child.Key };
            child.Bind(config);
            yield return config;
        }
    }
}
