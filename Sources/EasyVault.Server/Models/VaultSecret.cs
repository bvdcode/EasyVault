using System.Text.Json.Serialization;

namespace EasyVault.Server.Models
{
    public class VaultSecret
    {
        [JsonPropertyName("keyId")]
        public Guid KeyId { get; set; }

        [JsonPropertyName("appName")]
        public string AppName { get; set; } = string.Empty;

        [JsonPropertyName("values")]
        public Dictionary<string, string> Values { get; set; } = [];

        [JsonPropertyName("allowedAddresses")]
        public string[] AllowedAddresses { get; set; } = [];

        [JsonPropertyName("allowedUserAgents")]
        public string[] AllowedUserAgents { get; set; } = [];
    }
}