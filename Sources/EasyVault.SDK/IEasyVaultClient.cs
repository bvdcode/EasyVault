using System.Collections.Generic;

namespace EasyVault.SDK
{
    /// <summary>
    /// Interface for the EasyVaultClient.
    /// </summary>
    public interface IEasyVaultClient
    {
        /// <summary>
        /// Gets the secrets object.
        /// </summary>
        /// <typeparam name="T">Type of the secrets object.</typeparam>
        /// <returns>Secrets object.</returns>
        T GetSecrets<T>();

        /// <summary>
        /// Gets the secrets dictionary.
        /// </summary>
        /// <returns>Dictionary of secrets.</returns>
        Dictionary<string, string> GetSecrets();
    }
}