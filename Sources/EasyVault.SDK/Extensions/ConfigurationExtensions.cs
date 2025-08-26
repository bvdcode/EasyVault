using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace EasyVault.SDK.Extensions
{
    /// <summary>
    /// Extensions for the IConfiguration interface to add secrets from the EasyVault API.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Adds secrets from the EasyVault API to the configuration through In-Memory collection.
        /// </summary>
        /// <param name="configuration">The configuration to add secrets to.</param>
        /// <param name="configurationKey">The configuration key for the API key.</param>
        /// <param name="throwIfError">Whether to throw an exception if no secrets are found.</param>
        /// <param name="ignoreInDevelopment">Whether to ignore secrets in development environment.</param>
        /// <returns>The updated configuration.</returns>
        /// <exception cref="ArgumentException">Thrown when the configuration key is not set or is not a valid GUID.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no secrets are found for the API key.</exception>
        public static ConfigurationManager AddSecrets(this ConfigurationManager configuration,
            string configurationKey = "VaultApiKey", bool throwIfError = true, bool ignoreInDevelopment = true)
        {
            string? keyValue = configuration[configurationKey];
            if (string.IsNullOrWhiteSpace(keyValue))
            {
                throw new ArgumentException(configurationKey, $"Configuration key '{configurationKey}' is not set.");
            }
            bool parsed = Guid.TryParse(keyValue, out Guid apiKey);
            if (!parsed)
            {
                throw new ArgumentException(configurationKey, $"Configuration key '{configurationKey}' is not a valid GUID.");
            }
            if (ignoreInDevelopment)
            {
                bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
                if (isDevelopment)
                {
                    return configuration;
                }
            }
            EasyVaultClient client = new EasyVaultClient("url", apiKey);
            try
            {
                Dictionary<string, string> secrets = client.GetSecrets();
                if (secrets == null || secrets.Count == 0)
                {
                    if (throwIfError)
                    {
                        throw new InvalidOperationException($"No secrets found for API key '{apiKey}'.");
                    }
                    return configuration;
                }

                var nullableSecrets = secrets.Select(kvp => new KeyValuePair<string, string?>(kvp.Key, kvp.Value));
                configuration.AddInMemoryCollection(nullableSecrets);
                return configuration;
            }
            catch (Exception)
            {
                if (throwIfError)
                {
                    throw;
                }
                else
                {
                    return configuration;
                }
            }
        }
    }
}
