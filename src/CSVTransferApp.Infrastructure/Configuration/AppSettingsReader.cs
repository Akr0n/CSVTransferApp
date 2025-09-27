using Microsoft.Extensions.Configuration;

namespace CSVTransferApp.Infrastructure.Configuration;

public class AppSettingsReader
{
    private readonly IConfiguration _configuration;

    public AppSettingsReader(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public T GetValue<T>(string key, T defaultValue = default!)
    {
        return _configuration.GetValue<T>(key) ?? defaultValue;
    }

    public string GetConnectionString(string name)
    {
        return _configuration.GetConnectionString(name) ?? throw new InvalidOperationException($"Connection string '{name}' not found");
    }

    public IConfigurationSection GetSection(string key)
    {
        return _configuration.GetSection(key);
    }

    public void BindSection<T>(string sectionName, T instance) where T : class
    {
        _configuration.GetSection(sectionName).Bind(instance);
    }

    public T GetSection<T>(string sectionName) where T : class, new()
    {
        var instance = new T();
        _configuration.GetSection(sectionName).Bind(instance);
        return instance;
    }

    public Dictionary<string, string> GetKeyValuePairs(string sectionName)
    {
        return _configuration.GetSection(sectionName)
            .GetChildren()
            .ToDictionary(x => x.Key, x => x.Value ?? string.Empty);
    }
}
