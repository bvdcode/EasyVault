using EasyVault.Server.Models;

namespace EasyVault.Server.Services
{
    public class MemoryVaultService() : IVault
    {
        public bool IsSealed => GetSealedStatus();

        private static readonly Dictionary<Guid, VaultSecret> _secrets = [];

        private static bool GetSealedStatus()
        {
            return _secrets.Count == 0;
        }

        public VaultSecret GetSecrets(Guid keyId)
        {
            if (IsSealed)
            {
                throw new InvalidOperationException("Vault is sealed. Cannot access secrets.");
            }
            if (keyId == Guid.Empty)
            {
                throw new ArgumentException("Key ID cannot be empty.", nameof(keyId));
            }
            if (_secrets.TryGetValue(keyId, out var secrets))
            {
                return secrets;
            }
            throw new KeyNotFoundException($"No secrets found for key ID: {keyId}");
        }

        public void Unseal(IEnumerable<VaultSecret> secrets)
        {
            if (secrets == null || !secrets.Any())
            {
                throw new ArgumentException("Secrets cannot be null or empty.", nameof(secrets));
            }
            foreach (var secret in secrets)
            {
                _secrets[secret.KeyId] = secret;
            }
        }
    }
}
