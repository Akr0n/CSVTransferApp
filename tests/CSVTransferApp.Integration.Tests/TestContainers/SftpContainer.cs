using Testcontainers.Container;

namespace CSVTransferApp.Integration.Tests.TestContainers;

public class SftpContainer : IAsyncDisposable
{
    private readonly IContainer _container;

    public SftpContainer()
    {
        _container = new ContainerBuilder()
            .WithImage("atmoz/sftp:latest")
            .WithPortBinding(22, true)
            .WithEnvironment("SFTP_USERS", "testuser:testpass:1001:100:upload")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(22))
            .Build();
    }

    public async Task StartAsync()
    {
        await _container.StartAsync();
    }

    public string GetHost()
    {
        return _container.Hostname;
    }

    public ushort GetPort()
    {
        return _container.GetMappedPublicPort(22);
    }

    public async Task StopAsync()
    {
        await _container.StopAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
