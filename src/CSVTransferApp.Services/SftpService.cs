using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace CSVTransferApp.Services;
public class SftpService : ISftpService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SftpService> _logger;
    private readonly ConcurrentDictionary<string, SftpClient> _clients;

    public SftpService(IConfiguration configuration, ILogger<SftpService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _clients = new ConcurrentDictionary<string, SftpClient>();
    }

    public async Task UploadFileAsync(string connectionName, Stream fileStream, string fileName)
    {
        var client = GetSftpClient(connectionName);
        var config = _configuration.GetSection($"SftpConnections:{connectionName}");
        var remotePath = $"{config["RemotePath"]?.TrimEnd('/')}/{fileName}";

        _logger.LogInformation("Uploading {FileName} to {RemotePath}", fileName, remotePath);

        await Task.Run(() => client.UploadFile(fileStream, remotePath));

        _logger.LogInformation("Upload completed: {FileName}", fileName);
    }

    public async Task<bool> TestConnectionAsync(string connectionName)
    {
        try
        {
            var client = await Task.Run(() => GetSftpClient(connectionName));
            return client.IsConnected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to test SFTP connection {ConnectionName}", connectionName);
            return false;
        }
    }

    public async Task<bool> FileExistsAsync(string connectionName, string fileName)
    {
        var client = GetSftpClient(connectionName);
        var config = _configuration.GetSection($"SftpConnections:{connectionName}");
        var remotePath = Path.Combine(config["RemotePath"] ?? "/", fileName);

        return await Task.Run(() => client.Exists(remotePath));
    }

    public async Task DeleteFileAsync(string connectionName, string fileName)
    {
        var client = GetSftpClient(connectionName);
        var config = _configuration.GetSection($"SftpConnections:{connectionName}");
        var remotePath = Path.Combine(config["RemotePath"] ?? "/", fileName);

        if (await FileExistsAsync(connectionName, fileName))
        {
            await Task.Run(() => client.DeleteFile(remotePath));
            _logger.LogInformation("Deleted file {FileName} from {ConnectionName}", fileName, connectionName);
        }
    }

    public void Dispose()
    {
        foreach (var client in _clients.Values)
        {
            try
            {
                if (client.IsConnected)
                    client.Disconnect();
                client.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing SFTP client");
            }
        }

        _clients.Clear();
    }

    private SftpClient GetSftpClient(string connectionName)
    {
        return _clients.GetOrAdd(connectionName, name =>
        {
            var config = _configuration.GetSection($"SftpConnections:{name}");
            var host = config["Host"];
            var portStr = config["Port"] ?? "22";
            if (!int.TryParse(portStr, out var port))
                throw new InvalidOperationException($"Invalid port number: {portStr}");
            var username = config["Username"] ?? throw new InvalidOperationException("Username not configured");

            SftpClient client;

            if (!string.IsNullOrEmpty(config["PrivateKeyPath"]))
            {
                var privateKey = new PrivateKeyFile(config["PrivateKeyPath"]);
                client = new SftpClient(host, port, username, privateKey);
            }
            else
            {
                client = new SftpClient(host, port, username, config["Password"]);
            }

            client.Connect();
            return client;
        });
    }
}
