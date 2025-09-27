// Services/SftpService.cs
public class SftpService
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
        var remotePath = Path.Combine(config["RemotePath"], fileName);
        
        _logger.LogInformation("Uploading {FileName} to {RemotePath}", fileName, remotePath);
        
        await Task.Run(() => client.UploadFile(fileStream, remotePath));
        
        _logger.LogInformation("Upload completed: {FileName}", fileName);
    }

    private SftpClient GetSftpClient(string connectionName)
    {
        return _clients.GetOrAdd(connectionName, name =>
        {
            var config = _configuration.GetSection($"SftpConnections:{name}");
            var host = config["Host"];
            var port = int.Parse(config["Port"]);
            var username = config["Username"];
            
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
