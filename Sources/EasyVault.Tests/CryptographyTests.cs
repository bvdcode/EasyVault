using EasyVault.Server.Models;
using EasyExtensions.Extensions;
using EasyVault.Server.Database;
using System.Security.Cryptography;

namespace EasyVault.Tests
{
    public class CryptographyTests
    {
        private Vault _vault;
        private List<VaultSecret> _testSecrets;
        private const string TestKey = "SuperSecretKey123!";

        [SetUp]
        public void Setup()
        {
            _vault = new Vault();

            // Create test secrets
            _testSecrets =
            [
                new()
                {
                    KeyId = Guid.NewGuid(),
                    AppName = "TestApp",
                    Values = new Dictionary<string, string>
                    {
                        { "username", "admin" },
                        { "password", "p@ssw0rd!" }
                    },
                    AllowedAddresses = ["192.168.1.1", "10.0.0.1"],
                    AllowedUserAgents = ["Chrome", "Firefox"]
                },
                new()
                {
                    KeyId = Guid.NewGuid(),
                    AppName = "AnotherApp",
                    Values = new Dictionary<string, string>
                    {
                        { "apiKey", "sk_test_abcdef123456" },
                        { "environment", "production" }
                    },
                    AllowedAddresses = ["127.0.0.1"],
                    AllowedUserAgents = ["PostmanRuntime"]
                }
            ];
        }

        [Test]
        public void EncryptSecrets_ShouldEncryptData_WhenValidInputProvided()
        {
            // Act
            _vault.EncryptSecrets(TestKey, _testSecrets);
            using (Assert.EnterMultipleScope())
            {
                // Assert
                Assert.That(_vault.SecretKeyHashSha512, Is.Not.Empty);
                Assert.That(_vault.EncryptedData, Is.Not.Empty);
            }
            Assert.That(_vault.EncryptedData, Is.Not.EqualTo(System.Text.Json.JsonSerializer.Serialize(_testSecrets)));
        }

        [Test]
        public void DecryptSecrets_ShouldDecryptData_WhenCorrectKeyProvided()
        {
            // Arrange
            _vault.EncryptSecrets(TestKey, _testSecrets);

            // Act
            var decryptedSecrets = _vault.DecryptSecrets(TestKey).ToList();

            // Assert
            Assert.That(decryptedSecrets, Has.Count.EqualTo(_testSecrets.Count));
            using (Assert.EnterMultipleScope())
            {
                // Verify first secret
                Assert.That(decryptedSecrets[0].Values["username"], Is.EqualTo("admin"));
                Assert.That(decryptedSecrets[0].Values["password"], Is.EqualTo("p@ssw0rd!"));
                Assert.That(decryptedSecrets[0].AllowedAddresses, Is.EquivalentTo(["192.168.1.1", "10.0.0.1"]));

                // Verify second secret
                Assert.That(decryptedSecrets[1].Values["apiKey"], Is.EqualTo("sk_test_abcdef123456"));
                Assert.That(decryptedSecrets[1].Values["environment"], Is.EqualTo("production"));
            }
        }

        [Test]
        public void DecryptSecrets_ShouldThrowException_WhenIncorrectKeyProvided()
        {
            // Arrange
            _vault.EncryptSecrets(TestKey, _testSecrets);

            // Act & Assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() =>
                _vault.DecryptSecrets("WrongKey"));
            Assert.That(ex.Message, Is.EqualTo("Invalid decryption key provided."));
        }

        [Test]
        public void EncryptSecrets_ShouldThrowException_WhenKeyIsNullOrEmpty()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                _vault.EncryptSecrets(string.Empty, _testSecrets));
            Assert.That(ex.Message, Does.Contain("Key cannot be null or empty"));

            ex = Assert.Throws<ArgumentException>(() =>
                _vault.EncryptSecrets(null!, _testSecrets));
            Assert.That(ex.Message, Does.Contain("Key cannot be null or empty"));
        }

        [Test]
        public void EncryptSecrets_ShouldThrowException_WhenSecretsIsNullOrEmpty()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                _vault.EncryptSecrets(TestKey, null!));
            Assert.That(ex.Message, Does.Contain("Secrets cannot be null or empty"));

            ex = Assert.Throws<ArgumentException>(() =>
                _vault.EncryptSecrets(TestKey, []));
            Assert.That(ex.Message, Does.Contain("Secrets cannot be null or empty"));
        }

        [Test]
        public void DecryptSecrets_ShouldThrowException_WhenKeyIsNullOrEmpty()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                _vault.DecryptSecrets(string.Empty));
            Assert.That(ex.Message, Does.Contain("Key cannot be null or empty"));

            ex = Assert.Throws<ArgumentException>(() =>
                _vault.DecryptSecrets(null!));
            Assert.That(ex.Message, Does.Contain("Key cannot be null or empty"));
        }

        [Test]
        public void DecryptSecrets_ShouldReturnEmptyCollection_WhenNoEncryptedData()
        {
            // Arrange
            _vault.SecretKeyHashSha512 = TestKey.SHA512();
            _vault.EncryptedData = string.Empty;

            // Act
            var result = _vault.DecryptSecrets(TestKey);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void EncryptThenDecrypt_ShouldPreserveComplexData()
        {
            // Arrange
            var complexSecrets = new List<VaultSecret>
            {
                new() {
                    KeyId = Guid.NewGuid(),
                    Values = new Dictionary<string, string>
                    {
                        { "connectionString", "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;" },
                        { "apiToken", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c" }
                    },
                    AllowedAddresses = ["*.example.com", "api.service.com"],
                    AllowedUserAgents = ["Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36"]
                }
            };

            // Act
            _vault.EncryptSecrets(TestKey, complexSecrets);
            var decryptedSecrets = _vault.DecryptSecrets(TestKey).ToList();

            // Assert
            Assert.That(decryptedSecrets, Has.Count.EqualTo(1));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(decryptedSecrets[0].KeyId, Is.EqualTo(complexSecrets[0].KeyId));
                Assert.That(decryptedSecrets[0].Values["connectionString"], Is.EqualTo(complexSecrets[0].Values["connectionString"]));
                Assert.That(decryptedSecrets[0].Values["apiToken"], Is.EqualTo(complexSecrets[0].Values["apiToken"]));
                Assert.That(decryptedSecrets[0].AllowedAddresses, Is.EquivalentTo(complexSecrets[0].AllowedAddresses));
                Assert.That(decryptedSecrets[0].AllowedUserAgents, Is.EquivalentTo(complexSecrets[0].AllowedUserAgents));
            }
        }

        [Test]
        public void EncryptSecrets_ShouldDefaultToAES256_WhenEncryptionAlgorithmNotSet()
        {
            // Act
            _vault.EncryptSecrets(TestKey, _testSecrets);

            // Assert
            // Assuming EncryptionAlgorithm.AES256 has a value of 1
            Assert.That(_vault.HashAlgorithm, Is.EqualTo(HashAlgorithmName.SHA256.Name));
        }
    }
}
