# Changelog

## v0.1.0 — First public preview

EasyVault is a lightweight secrets service that focuses on the essentials: encrypt data, store it safely, and fetch it simply.
This first public preview delivers a minimal-yet-solid foundation for local use and small services.

### Highlights

- Simple REST API with encrypted-at-rest storage (AES‑256 with PBKDF2‑SHA256; only a SHA‑512 of the password is stored).
- “Unseal” model backed by in‑memory cache: decrypt once with your password, then fetch by `keyId` while the process stays warm.
- Access controls per secret via IP patterns and User‑Agent patterns (with wildcard support).
- SQLite by default; automatic EF Core migrations on startup.
- Health checks exposed at `/api/v1/health`.
- Small .NET SDK: get a `Dictionary<string,string>` or map to a typed config object (case‑insensitive by property name).

### API (short overview)

- POST `/api/v1/vault/{password}` — Encrypt and persist an array of `VaultSecret`.
- GET `/api/v1/vault/{password}` — Decrypt latest vault with this password and unseal the in‑memory cache.
- GET `/api/v1/vault/secrets/{keyId}?format=json|plain` — Return values for a single `keyId` if unsealed and allowed by IP/UA.
- Health: `/api/v1/health`.

`VaultSecret` shape:

```json
{
  "keyId": "b7c1e6b6-4c5a-4c9c-9d31-0bb2d6bf9b4a",
  "appName": "MyApp",
  "values": { "DB_PASSWORD": "supersecret", "API_TOKEN": "abcdef" },
  "allowedAddresses": ["127.0.0.1", "10.0.*"],
  "allowedUserAgents": ["PostmanRuntime", "MyService/*"]
}
```

### Quick start

Run locally without Docker:

```powershell
cd .\Sources\EasyVault.Server
dotnet run
```

Local Docker build:

```powershell
# build the image
docker build -t easyvault:local -f .\Sources\EasyVault.Server\Dockerfile .\Sources

# run the container
docker run --name easyvault -p 8080:8080 -v easyvault_data:/data easyvault:local
```

Configuration is in `Sources/EasyVault.Server/appsettings.json` and can be overridden via ASP.NET Core environment variables. CORS origins are controlled by `AllowedOrigins`.

### .NET SDK example

```csharp
using EasyVault.SDK;

var client = new EasyVaultClient("http://localhost:8080", new Guid("b7c1e6b6-4c5a-4c9c-9d31-0bb2d6bf9b4a"));

// Raw key-value map
var map = client.GetSecrets();

// Typed mapping by property names (case-insensitive)
var typed = client.GetSecrets<MySecrets>();

public class MySecrets
{
    public string DB_PASSWORD { get; set; } = string.Empty;
    public string API_TOKEN   { get; set; } = string.Empty;
}
```

### You have to know

- Unseal state is in‑memory; it resets on restart and isn’t shared across instances.
- No multi‑tenant isolation or RBAC; access relies on possession of the password and per‑secret IP/UA rules.
- Only SQLite is wired in this repo; external DBs are not included.

### What’s next

- Optional request signing (HMAC).
- Packaging and deployment docs (container registry, Helm chart).

— Enjoy, and please report issues or ideas!
