# 🛡️ LYRA — Let Yourself Remain Authenticated

---

## What is LYRA?

**LYRA** is a self-hosted authorization system for verifying **signed requests** between **trusted systems**.  
It ensures that each request across service or company boundaries is intentional, validated, and safe — without inspecting the business payload.

---

## Project Overview

| Component       | Role                                                                 |
|----------------|----------------------------------------------------------------------|
| `LYRA.Server`   | Central verifier for signed requests. Enforces access policies.       |
| `LYRA.Client`   | SDK for generating and signing requests from trusted systems.         |
| `LYRA.Security` | Cryptographic core: signing, hashing, contracts, and string builders. |

---

##  How It Works

1. A sender system builds a `GenericMetadata` object:
   - Includes `caller`, `target`, `action`, `resource`, `payloadHash`, `timestamp`
2. It signs the canonical string using HMAC or RSA.
3. It sends a `VerifyRequest` to `LYRA.Server`
4. `LYRA.Server` performs:
   - Signature validation using the appropriate secret or public key
   - Policy check based on `AccessPolicy`
   - Optional payload hash recomputation
   - Returns `VerifyResponse` indicating success/failure

---

## Sample Flow

### VerifyRequest

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

## Canonicalization Example

```csharp
SignatureStringBuilder.BuildStringToSign(metadata);
// Returns:
"caller=...&target=...&action=...&resource=...&payloadHash=...&timestamp=..."
```

- No percent-encoding; values must be normalized.
- `payloadHash` is SHA‑512 of raw payload, Base64 encoded.

---

## LYRA.Server Highlights

- ASP.NET Core 8 + Razor Pages
- SQL Server + EF Core
- SignalR log console
- Denormalized cache for fast policy lookup
- SaveChangesInterceptor for real-time sync
- HMAC / RSA support via `LYRA.Security`

---

## LYRA.Client Highlights

- SDK for generating `GenericMetadata` and `SignedMetadata`
- Signs request using HMAC or RSA
- Can generate `VerifyRequest` or just headers
- Works in APIs, workers, gateways
- Multi-touchpoint support (apps with multiple identities)

```csharp
var signer = new LyraSigner(touchpoint);
var signed = signer.Sign(metadata);
```

---

## LYRA.Security Highlights

- Pure C# 12 / .NET 8, no third-party deps
- `GenericMetadata`, `SignedMetadata`, `VerifyRequest`
- `EncryptionHelper` for SHA‑512, HMAC, SecureEquals
- `Signer.Sign(...)`, `Signer.Verify(...)`
- `SignatureStringBuilder.BuildStringToSign(...)`
- Strict, deterministic signing

---

## Security Recommendations

- Use UTC timestamps in Unix format
- Enforce max allowed time drift (e.g. ±2h)
- Use constant-time comparison for all signature checks
- Prefer per-touchpoint secrets with rotation
- Normalize and validate paths, actions, and payloads

---

## Integration Tips

- Add signed values to HTTP headers (for REST)
- Use LYRA.Client to sign all inter-system requests
- Use `/api/verify` endpoint in LYRA.Server to validate
- Store all secrets encrypted at rest

---

## Tech Stack

- .NET 8 (C# 12)
- ASP.NET Core Razor Pages + API
- SQL Server + EF Core
- SignalR
- System.Security.Cryptography

---

## License

MIT License — see [LICENSE](LICENSE).

---

**🛡️ LYRA. Let Yourself Remain Authenticated.**
