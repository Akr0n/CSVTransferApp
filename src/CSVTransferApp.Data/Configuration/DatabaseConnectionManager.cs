using System.Collections.Concurrent;
using System.Data;
using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Core.Exceptions;
using CSVTransferApp.Data.Factories;

namespace CSVTransferApp.Data.Configuration;

public class DatabaseConnectionManager : IDisposable
{
    private readonly IConfigurationService _configurationService;
    private readonly ConnectionFactory _connectionFactory;
    private readonly ConcurrentDictionary<string, IDbConnection> _activeConnections;
    private readonly object _lockObject = new();

    public DatabaseConnectionManager(IConfigurationService configurationService, ConnectionFactory connectionFactory)
    {
        _configurationService = configurationService;
        _connectionFactory = connectionFactory;
        _activeConnections = new ConcurrentDictionary<string, IDbConnection>();
    }

    public async Task<IDbConnection> GetConnectionAsync(string connectionName)
    {
        if (_activeConnections.TryGetValue(connectionName, out var existingConnection) 
            && existingConnection.State == ConnectionState.Open)
        {
            return existingConnection;
        }

        lock (_lockObject)
        {
            if (_activeConnections.TryGetValue(connectionName, out var connection) 
                && connection.State == ConnectionState.Open)
            {
                return connection;
            }

            try
            {
                var config = _configurationService.GetDatabaseConnection(connectionName);
                var newConnection = _connectionFactory.CreateConnection(config);
                
                newConnection.Open();
                _activeConnections.TryAdd(connectionName, newConnection);
                
                return newConnection;
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException(connectionName, $"Failed to establish connection: {ex.Message}", ex);
            }
        }
    }

    public async Task<bool> TestConnectionAsync(string connectionName)
    {
        try
        {
            var config = _configurationService.GetDatabaseConnection(connectionName);
            using var connection = _connectionFactory.CreateConnection(config);
            await Task.Run(() => connection.Open());
            return connection.State == ConnectionState.Open;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        foreach (var connection in _activeConnections.Values)
        {
            try
            {
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                connection.Dispose();
            }
            catch { /* Ignore disposal errors */ }
        }
        
        _activeConnections.Clear();
        GC.SuppressFinalize(this);
    }
}
