namespace CSVTransferApp.Core.Models;

public class SftpConnectionConfig
{
    public string Name { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 22;
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? PrivateKeyPath { get; set; }
    public string? Passphrase { get; set; }
    public string RemotePath { get; set; } = "/";
    public int ConnectionTimeout { get; set; } = 30;
    public bool IsEnabled { get; set; } = true;
}
