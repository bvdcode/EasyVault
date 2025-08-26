namespace EasyVault.Server.Services
{
    public class MemoryVaultService(INotificationSender _notifications) : IVault
    {
        public bool IsSealed => GetSealedStatus();

        private static readonly Dictionary<Guid, VaultSecret> _secrets = [];
        private static bool notificationState = true;

        private bool GetSealedStatus()
        {
            bool isSealed = _secrets.Count == 0;
            if (isSealed && notificationState)
            {
                _notifications.SendNotificationAsync("Vault", "Sealed!", isError: true)
                    .ContinueWith((t) =>
                    {
                        if (t.IsCompleted && t.Exception == null)
                        {
                            notificationState = false;
                        }
                    });
            }
            return isSealed;
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
            if (!notificationState)
            {
                _notifications.SendNotificationAsync("Vault", "Vault is unsealed. Secrets are now accessible.", isError: false);
                notificationState = true;
            }
        }
    }
}
