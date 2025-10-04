using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace CSVTransferApp.Infrastructure.Configuration;

public class EnvironmentVariableConfigurationProvider : ConfigurationProvider
{
    private readonly string _prefix;
    private readonly bool _skipUnresolved;
    private static readonly Regex _placeholderRegex = new(@"\${(\w+)}", RegexOptions.Compiled);

    public EnvironmentVariableConfigurationProvider(string prefix = "", bool skipUnresolved = false)
    {
        _prefix = prefix;
        _skipUnresolved = skipUnresolved;
    }

    public override void Load()
    {
        foreach (var kvp in Data)
        {
            var value = kvp.Value;
            if (string.IsNullOrEmpty(value)) continue;

            var resolvedValue = ResolvePlaceholders(value);
            if (resolvedValue != value)
            {
                Data[kvp.Key] = resolvedValue;
            }
        }
    }

    private string ResolvePlaceholders(string value)
    {
        return _placeholderRegex.Replace(value, match =>
        {
            var envVarName = match.Groups[1].Value;
            var prefixedName = string.IsNullOrEmpty(_prefix) ? envVarName : $"{_prefix}_{envVarName}";
            
            var envValue = Environment.GetEnvironmentVariable(prefixedName);
            if (envValue == null)
            {
                if (_skipUnresolved)
                    return match.Value;
                
                throw new InvalidOperationException(
                    $"Environment variable '{prefixedName}' not found for configuration placeholder");
            }
            
            return envValue;
        });
    }
}