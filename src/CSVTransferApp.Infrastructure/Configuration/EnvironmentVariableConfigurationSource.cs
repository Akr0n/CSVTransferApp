using Microsoft.Extensions.Configuration;

namespace CSVTransferApp.Infrastructure.Configuration;

public class EnvironmentVariableConfigurationSource : IConfigurationSource
{
    private readonly string _prefix;
    private readonly bool _skipUnresolved;

    public EnvironmentVariableConfigurationSource(string prefix = "", bool skipUnresolved = false)
    {
        _prefix = prefix;
        _skipUnresolved = skipUnresolved;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new EnvironmentVariableConfigurationProvider(_prefix, _skipUnresolved);
    }
}