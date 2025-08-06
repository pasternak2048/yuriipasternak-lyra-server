# 🛡️LYRA. Let Yourself Remain Authenticated.
---

## What is LYRA?

**LYRA** is a self-hosted authorization system for verifying **signed requests** between **trusted systems**.  
It ensures that each request across service or company boundaries is intentional, validated, and safe — without inspecting the business payload.

---

## What is LYRA.Server?

**LYRA.Server** is the central verifier in the LYRA ecosystem. It checks incoming signed requests using cryptographic validation and enforced access rules.

- Receives and verifies `VerifyRequest` structures from clients or systems
- Validates signatures (HMAC, RSA)
- Resolves `AccessPolicy` between known systems (Trusted Touchpoints)
- Stores structured logs and enforces replay/time-based protections

---

## How it Works

1. A sender system builds a `GenericMetadata` object:
   - `caller`, `target`, `action`, `resource`, `payloadHash`, `timestamp`
2. It signs the canonical string and sends a `VerifyRequest`
3. LYRA.Server:
   - Looks up the caller and target
   - Validates the signature using the correct secret or key
   - Confirms the access is allowed by policy
   - Returns a `VerifyResponse` indicating success or failure

---

## Core Concepts

| Concept              | Description |
|----------------------|-------------|
| `Company`            | Represents an organization (owns touchpoints) |
| `TrustedTouchpoint`  | A named system component with a secret/key |
| `AccessPolicy`       | Rule granting permission from one touchpoint to another |
| `SignatureType`      | Algorithm used (HMAC, RSA) |
| `GenericMetadata`    | Canonical fields used for signing |
| `VerifyRequest`      | Payload sent to `/api/verify` |
| `VerifyResponse`     | Result: success/failure + optional reason |

---

## Authorization Flow Example

```json
POST /api/verify
{
  "metadata": {
    "caller": "gateway@bcorp",
    "target": "billing@acorp",
    "action": "post",
    "resource": "/subscribe",
    "payloadHash": "sha512:...",
    "timestamp": "1723123523"
  },
  "signed": {
    "type": "HmacSha512",
    "value": "base64..."
  }
}
```

---

## Architecture Highlights

- ✅ Built-in `SaveChangesInterceptor` to auto-update cache
- ✅ Denormalized `CachedAccessPolicy` for lightning-fast lookups
- ✅ In-memory cache for hot-path verification
- ✅ Razor Pages UI for managing Companies, Touchpoints, and Policies
- ✅ Live console viewer for security logs via SignalR
- ✅ Supports `HMAC`, `RSA`, and extensible signature types

---

## Tech Stack

- ASP.NET Core 8 + Razor Pages
- Entity Framework Core + SQL Server
- SignalR for live streaming logs
- `Microsoft.Extensions.Caching.Memory`
- `System.Security.Cryptography`
- LYRA.Security (for signing contracts and validation helpers)

---

## Used by

- **[LYRA.Client](https://github.com/pasternak2048/yuriipasternak-lyra-client)** for signing and sending
- **[LYRA.Security](https://github.com/pasternak2048/yuriipasternak-lyra-security)** for shared models and crypto
- Can be extended for gRPC / EventBus / Custom flows

---

## License

Licensed under the [MIT License](LICENSE).

**🛡️LYRA. Let Yourself Remain Authenticated.**
