using System;
using Microsoft.Extensions.Configuration;

namespace Makelaar.Helpers;

public static class ConfigurationExtension
{
    public static string RetrieveConfigurationValue(this IConfiguration configuration, string configurationName)
    {
        var configValue = configuration[configurationName];
        if (string.IsNullOrWhiteSpace(configValue))
        {
            throw new ArgumentNullException(configurationName, $"Configuration value for '{configurationName}' is missing or empty.");
        }

        return configValue;
    }
}
