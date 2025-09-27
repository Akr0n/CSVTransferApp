// Program.cs
public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        var services = new ServiceCollection();
        ConfigureServices(services, configuration);
        
        using var serviceProvider = services.BuildServiceProvider();
        var app = serviceProvider.GetRequiredService<CsvTransferApplication>();
        
        return await app.RunAsync(args);
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddLogging(builder => builder.AddConsole().AddFile("logs/app-.log"));
        services.AddSingleton<DatabaseService>();
        services.AddSingleton<SftpService>();
        services.AddSingleton<CsvProcessingService>();
        services.AddSingleton<CsvTransferApplication>();
    }
}
