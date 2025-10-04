using Microsoft.Extensions.Configuration;

namespace CSVTransferApp.Infrastructure.Configuration;

public static class EnvironmentVariableConfigurationExtensions
{
    public static IConfigurationBuilder AddEnvironmentVariablePlaceholderResolver(
        this IConfigurationBuilder builder,
        string prefix = "",
        bool skipUnresolved = false)
    {
        builder.Add(new EnvironmentVariableConfigurationSource(prefix, skipUnresolved));
        return builder;
    }
}