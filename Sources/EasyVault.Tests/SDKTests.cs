using EasyVault.SDK;

namespace EasyVault.Tests
{
    public class SDKTests
    {
        private const string baseUrl = "https://easyvault.example.com";
        private const string testApiKey = "a8a7bee1-1234-1234-1234-38d173a5a6fe";
        private const string remoteKey = "TestKey";
        private const string remoteValue = "TestValue";

        [Test]
        public void GetSecrets_ShouldReturnSecrets_WhenValidApiKeyProvided()
        {
            // Arrange
            var client = new EasyVaultClient(baseUrl, Guid.Parse(testApiKey));
            var expected = new Dictionary<string, string>
            {
                { remoteKey, remoteValue }
            };
            // Act
            var result = client.GetSecrets();
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetSecretsGeneric_ShouldReturnTypedSecrets_WhenValidApiKeyProvided()
        {
            // Arrange
            var client = new EasyVaultClient(baseUrl, Guid.Parse(testApiKey));

            // Act
            var result = client.GetSecrets<TestSecretConfig>();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TestKey, Is.EqualTo(remoteValue));
        }

        [Test]
        public void Constructor_ShouldThrowArgumentException_WhenEmptyApiKeyProvided()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new EasyVaultClient(baseUrl, Guid.Empty));
        }

        [Test]
        public void Constructor_ShouldAcceptCustomBaseUrl()
        {
            // Arrange
            var customUrl = "https://custom.vault.example.com";

            // Act
            // Note: This test is limited since we can't easily verify the internal HttpClient's BaseAddress
            var client = new EasyVaultClient(customUrl, Guid.Parse(testApiKey));

            // Assert
            Assert.That(client, Is.Not.Null);
            // The fact that it didn't throw an exception is part of the test
        }

        [Test]
        public void GetSecretsGeneric_ShouldMapPropertiesCaseInsensitive()
        {
            // Arrange
            var client = new EasyVaultClient(baseUrl, Guid.Parse(testApiKey));

            // Act
            var result = client.GetSecrets<TestSecretConfigDifferentCase>();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.testkey, Is.EqualTo(remoteValue));
        }

        public class TestSecretConfig
        {
            public string TestKey { get; set; } = string.Empty;
        }

        public class TestSecretConfigDifferentCase
        {
#pragma warning disable IDE1006 // Naming Styles
            public string testkey { get; set; } = string.Empty;
#pragma warning restore IDE1006 // Naming Styles
        }
    }
}