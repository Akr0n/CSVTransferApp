using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using CSVTransferApp.Core.Models;
using CSVTransferApp.Core.Interfaces;
using CSVTransferApp.Core.Exceptions;
using CSVTransferApp.Data.Factories;

namespace CSVTransferApp.Data.Configuration;

public class DatabaseConnectionManager : IDatabaseConnectionManager, IDisposable
{
    public void ReleaseConnection(string connectionName)
    {
        if (string.IsNullOrEmpty(connectionName))
            throw new ArgumentException("Connection name cannot be null or empty", nameof(connectionName));
            
        if (_activeConnections.TryRemove(connectionName, out var connection))
        {
            try
            {
                connection.Dispose();
            }
            catch (Exception ex)
            {
                // Log error but don't throw - we've already removed it from active connections
                Debug.WriteLine($"Error disposing connection {connectionName}: {ex.Message}");
            }
        }
    }

    private readonly IConfigurationService _configurationService;
    private readonly IConnectionFactory _connectionFactory;
    private readonly ConcurrentDictionary<string, IDbConnection> _activeConnections;
    private readonly object _lockObject = new();

    public DatabaseConnectionManager(IConfigurationService configurationService, IConnectionFactory connectionFactory)
    {
        _configurationService = configurationService;
        _connectionFactory = connectionFactory;
        _activeConnections = new ConcurrentDictionary<string, IDbConnection>();
    }

    public async Task<IDbConnection> GetConnectionAsync(string connectionName)
    {
        // Primo controllo senza lock
        if (_activeConnections.TryGetValue(connectionName, out var existingConnection)
            && existingConnection.State == ConnectionState.Open)
        {
            return existingConnection;
        }

        IDbConnection? connection = null;

        // Se serve creare una connessione, acquisisci il lock solo per le operazioni sul dizionario
        lock (_lockObject)
        {
            if (_activeConnections.TryGetValue(connectionName, out var conn) 
                && conn.State == ConnectionState.Open)
            {
                return conn;
            }

            var config = _configurationService.GetDatabaseConnection(connectionName);
            connection = _connectionFactory.CreateConnection(config.ConnectionString, config.Provider);

            // Non aprire la connessione qui! Solo aggiungi al dizionario se vuoi il comportamento early registration:
            _activeConnections.TryAdd(connectionName, connection);
        }

        // Fuori dal lock: apri asincrono la connessione
        if (connection is DbConnection dbConn)
        {
            await dbConn.OpenAsync();
            return dbConn;
        }
        else
        {
            // Per provider che non supportano OpenAsync
            connection.Open();
            return connection;
        }
    }

    public async Task<bool> TestConnectionAsync(string connectionName)
    {
        try
        {
            var config = _configurationService.GetDatabaseConnection(connectionName);
            using var connection = _connectionFactory.CreateConnection(config.ConnectionString, config.Provider);
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
