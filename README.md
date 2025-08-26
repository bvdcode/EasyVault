![CI](https://github.com/bvdcode/EasyVault/actions/workflows/publish-release.yml/badge.svg)
[![License](https://img.shields.io/github/license/bvdcode/EasyVault)](LICENSE)

# EasyVault

Lightweight secrets storage service for developers: simple REST API, encrypted data storage in DB and minimal SDK for .NET.

## What's inside

- AES-256 (PBKDF2-SHA256) for data encryption in DB; key is not stored (only SHA-512 password hash is saved).
- IP and User-Agent based access restrictions for secrets.
- SQLite by default, migrations are applied automatically.
- Health-check at `/api/v1/health` endpoint.
- Simple .NET SDK (Dictionary and typed property mapping).

## Quick start

In Docker (locally):

```powershell
# Build image
docker build -t easyvault:local -f .\Sources\EasyVault.Server\Dockerfile .\Sources

# Run container
docker run --name easyvault -p 8080:8080 -v easyvault_data:/data easyvault:local
```

Local run without Docker:

```powershell
cd .\Sources\EasyVault.Server
dotnet run
```

By default, server listens on port `8080`. DB path: `/data/easyvault.db` (for Docker run).

## Configuration

Settings in `Sources/EasyVault.Server/appsettings.json`:

```json
{
  "SqliteConnectionString": "Data Source=/data/easyvault.db;Cache=Shared;Foreign Keys=True;Pooling=True;Mode=ReadWriteCreate;",
  "AllowedOrigins": [
    "http://localhost:5173",
    "https://vault.company.name"
  ]
}
```

These values can be overridden with ASP.NET Core environment variables.

## API (brief)

1) Create/update "vault" (encrypt and save):

```
POST /api/v1/vault/{password}
Content-Type: application/json

[
  {
    "keyId": "b7c1e6b6-4c5a-4c9c-9d31-0bb2d6bf9b4a",
    "appName": "MyApp",
    "values": { "DB_PASSWORD": "supersecret", "API_TOKEN": "abcdef" },
    "allowedAddresses": ["127.0.0.1", "10.0.*"],
    "allowedUserAgents": ["PostmanRuntime", "MyService/*"]
  }
]
```

2) Get secrets by `keyId` (access granted by IP/UA; "vault" must be "unsealed" after request to step 1):

```
GET /api/v1/vault/secrets/{keyId}?format=json|plain
```

- `format=json` — returns `{ "KEY": "VALUE" }`.
- `format=plain` — returns strings like `KEY=VALUE`.

Note: password (`{password}`) is used only for encryption/decryption and is not stored in DB (only SHA-512 hash is stored).

## .NET SDK

Client usage example:

```csharp
using EasyVault.SDK;

var client = new EasyVaultClient("http://localhost:8080", new Guid("b7c1e6b6-4c5a-4c9c-9d31-0bb2d6bf9b4a"));

// Key-value dictionary
var map = client.GetSecrets();

// Typed mapping by property names (case-insensitive)
var typed = client.GetSecrets<MySecrets>();

public class MySecrets
{
    public string DB_PASSWORD { get; set; } = string.Empty;
    public string API_TOKEN { get; set; } = string.Empty;
}
```

## Development

- Server: `Sources/EasyVault.Server` — ASP.NET Core 9.0, SQLite.
- Web client (optional): `Sources/easyvault.client` — Vite + React.
- Tests: `Sources/EasyVault.Tests`.

```powershell
cd .\Sources
dotnet build
dotnet test

# frontend (optional)
cd .\easyvault.client; npm i; npm run dev
```

## License

MIT. See [LICENSE](LICENSE) file.
