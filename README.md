# EasyVault

> A pragmatic secrets bag for developers who just want it to work.

**EasyVault** is a lightweight secrets storage service. No endless unseal steps, no enterprise overkill, no GitLab-variable mess.
Just a simple REST API + SDK that lets you encrypt, store and fetch application secrets with minimal friction.

---

## ✨ Features

- 🚀 **Zero setup pain** – single container, Postgres or SQLite inside.
- 🔑 **Client-side encryption** – secrets are encrypted before hitting the DB; server never sees plaintext.
- 🌐 **Access rules** – restrict by IP ranges, User-Agent, or extra headers.
- 🛠 **NuGet SDK** – drop into .NET projects, async API, DI-friendly.
- 📦 **Batteries included** – migrations, health checks, simple ENV config.
- 🐒 **No bullshit** – no plugins, no manual unseals, no 50-page manuals.

---

## 🛠 Quick start

### Docker Compose

```yaml
services:
  easyvault:
    image: bvdcode/easyvault:latest
    volumes:
      - ev_data:/data
```

Bring it up:

```bash
docker compose up -d
```

---

## 🔑 Using the API

### Add a secret

```http
POST /api/v1/vault/my-key
Content-Type: application/json

{
  "secrets": {
    "DB_PASSWORD": "supersecret",
    "API_TOKEN": "abcdef123"
  }
}
```

### Fetch secrets

```http
GET /api/v1/vault/secrets/my-key
```

Response:

```json
{
  "DB_PASSWORD": "supersecret",
  "API_TOKEN": "abcdef123"
}
```

Access is checked against your IP / UA / headers config.

---

## 📦 SDK for .NET

Install via NuGet:

```bash
dotnet add package EasyVault.SDK
```

Usage:

```csharp
var client = new EasyVaultClient("https://vault.mycompany.dev", "my-key");
var secrets = await client.GetSecretsAsync();
Console.WriteLine(secrets["DB_PASSWORD"]);
```

---

## ⚡ Roadmap

- [ ] Optional PSK request signing (HMAC)
- [ ] Audit logs (without secrets)
- [ ] Admin UI for quick edits
- [ ] Helm chart

---

## 📜 License

MIT.
