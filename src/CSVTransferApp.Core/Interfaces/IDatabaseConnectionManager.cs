using System.Data;

namespace CSVTransferApp.Core.Interfaces;

public interface IDatabaseConnectionManager
{
    Task<IDbConnection> GetConnectionAsync(string connectionName);
    void ReleaseConnection(string connectionName);
}