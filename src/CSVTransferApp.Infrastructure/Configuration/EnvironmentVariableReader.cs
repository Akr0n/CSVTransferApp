namespace CSVTransferApp.Infrastructure.Configuration;

public class EnvironmentVariableReader
{
    public string? GetEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable(name);
    }

    public T GetEnvironmentVariable<T>(string name, T defaultValue = default!)
    {
        var value = Environment.GetEnvironmentVariable(name);
        
        if (string.IsNullOrEmpty(value))
            return defaultValue;

        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }

    public Dictionary<string, string> GetEnvironmentVariablesWithPrefix(string prefix)
    {
        return Environment.GetEnvironmentVariables()
            .Cast<System.Collections.DictionaryEntry>()
            .Where(entry => entry.Key.ToString()!.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(
                entry => entry.Key.ToString()!,
                entry => entry.Value?.ToString() ?? string.Empty
            );
    }

    public void SetEnvironmentVariable(string name, string value)
    {
        Environment.SetEnvironmentVariable(name, value);
    }

    public bool HasEnvironmentVariable(string name)
    {
        return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name));
    }
}
