# 🛡 LYRA - Let Yourself Remain Authenticated

---

## What is LYRA?

**LYRA** is a self-hosted verification and authorization system for **signed requests** between **trusted systems**.

It is designed for scenarios where one internal service, gateway, worker, or partner system needs to prove:

- who sent the request
- who the request is intended for
- what operation is being requested
- that the payload was not modified
- that the request is allowed by policy

LYRA verifies those facts without interpreting the business meaning of the payload itself.

---

## Solution Structure

| Component | Role |
|---|---|
| `LYRA.Server` | Central verification server with API, admin UI, policy management, caching, logging, and identity. |
| `LYRA.Client` | SDK for generating signed metadata and sending verification requests to `LYRA.Server`. |
| `LYRA.Security` | Shared contracts and cryptographic primitives used by both client and server. |
| [`MILANO`](https://github.com/pasternak2048/yuriipasternak-milano-distributedcache) | External distributed cache used for policy lookup acceleration and replay protection. |

---

## Current Tech Stack

- .NET 10
- ASP.NET Core Razor Pages + Web API
- ASP.NET Core Identity
- Entity Framework Core + SQL Server
- SignalR
- `System.Security.Cryptography`
- `MILANO.Client`

---

## How Verification Works

### 1. The caller builds metadata

The sender creates `GenericMetadata` with these fields:

- `callerSystemName`
- `targetSystemName`
- `method`
- `path`
- `bodyHash`
- `timestamp`

### 2. The caller signs the canonical string

The canonical string is built in a fixed order:

```csharp
SignatureStringBuilder.BuildStringToSign(metadata);
```

Example output:

```text
caller=default-tp-01@company-01&target=default-tp-02@company-02&method=POST&path=/api/verification/verify&bodyHash=...&timestamp=...
```

### 3. The caller sends `VerifyRequest`

The request contains:

- `metadata`
- `signed`
- optional raw `payload`
- optional `requestId` for replay protection

### 4. `LYRA.Server` verifies the request

The server:

- validates timestamp drift
- checks replay protection using `requestId`
- loads cached access policy for `caller -> target`
- checks method/path rules
- recomputes payload SHA-512 hash
- verifies the signature using the caller secret
- returns `VerifyResponse`

---

## Current API Example

### Verification endpoint

```http
POST /api/verification/verify
```

### Example `VerifyRequest`

```json
{
  "metadata": {
    "callerSystemName": "default-tp-01@company-01",
    "targetSystemName": "default-tp-02@company-02",
    "method": "POST",
    "path": "/api/verification/verify",
    "bodyHash": "Pbj2qQ5TZA3BCfALJxWZG2VQWvQSM3i8ri1pJ+7OeJzD4Q5c9kLSuN1hWn8tgonM8kJ6V0WQeSRbYlJ2b0F4ZQ==",
    "timestamp": "1776988800"
  },
  "signed": {
    "signatureType": "HmacSha512",
    "signature": "base64-signature"
  },
  "payload": "{\"demo\":\"hello\"}",
  "requestId": "req-001"
}
```

### Example success response

```json
{
  "success": true
}
```

### Example failure response

```json
{
  "success": false,
  "reason": "PolicyDenied",
  "details": "Access denied: no policy or disabled"
}
```

---

## Core Concepts

| Concept | Description |
|---|---|
| `Company` | Organization-level owner of touchpoints. |
| `TrustedTouchpoint` | A trusted caller/receiver identity with secret and signature type. |
| `AccessPolicy` | Permission linking one caller touchpoint to one target touchpoint. |
| `AccessPolicyRule` | Allowed HTTP method + path pattern for a policy. |
| `GenericMetadata` | Shared canonical request metadata used for signing. |
| `SignedMetadata` | Signature type and Base64 signature. |
| `VerifyRequest` | Request payload sent to `LYRA.Server`. |
| `VerifyResponse` | Verification result with optional failure reason/details. |
| `CachedAccessPolicy` | Flattened policy record used for fast verification lookups. |

---

## Supported Signature Algorithms

Current implementation supports:

- `HmacSha256`
- `HmacSha512`

`HmacSha512` is the default in seeded data and the main expected path.

RSA/ECDSA/Ed25519 are not currently implemented in the codebase.

---

## Request Matching Rules

LYRA normalizes and checks:

- HTTP method
- request path
- body hash
- timestamp
- caller/target pair

Path rules support:

- exact match, for example `/api/orders/create`
- prefix wildcard, for example `/api/orders/*`
- full wildcard `/*`

Method rules support:

- exact verbs like `GET`, `POST`, `PUT`, `DELETE`
- wildcard via `*`
- `ANY`, normalized to `*`

---

## LYRA.Server Highlights

- Razor Pages admin dashboard
- verification API controller
- ASP.NET Core Identity-based login
- SQL-backed main database
- separate cached database for flattened access policies
- separate logs database
- SignalR live activity stream
- distributed cache integration through MILANO
- replay protection store
- automatic migrations + seed on startup

### Main managed areas in the dashboard

- Companies
- Trusted Touchpoints
- Access Policies
- Logging

---

## LYRA.Client Highlights

- generates deterministic signatures using shared contracts
- resolves signer identity from configured touchpoints
- can be registered through DI
- can call `LYRA.Server` verification endpoint over `HttpClient`

Typical signing flow:

```csharp
var result = lyraClient.GenerateSignedMetadata(metadata);
```

This returns:

- `SignedMetadata`
- the canonical `StringToSign` for diagnostics

---

## LYRA.Security Highlights

- shared models used by both server and client
- deterministic canonical string generation
- signature abstraction through `Signer`
- HMAC-SHA256 and HMAC-SHA512 implementations
- hashing and constant-time comparison helpers

Main types:

- `GenericMetadata`
- `SignedMetadata`
- `VerifyRequest`
- `VerifyResponse`
- `SignatureStringBuilder`
- `Signer`

---

## Caching Model

LYRA uses two cache layers for authorization data:

### SQL cached database

The server stores denormalized policy data in a dedicated cached database:

- caller/target names
- serialized rules
- caller secret
- signature type
- caller/target company names

### [MILANO](https://github.com/pasternak2048/yuriipasternak-milano-distributedcache) — Distributed Cache

MILANO is used for:

- hot policy lookups
- replay protection keys

This keeps verification fast and avoids repeated joins over the main relational model.

---

## Logging Model

Verification and system events are:

- queued in memory
- persisted into the logs database by a background service
- broadcast live to connected dashboard clients through SignalR

Typical log categories include:

- verification success/failure
- database seed/migration events
- admin operations
- unexpected exceptions

---

## Databases

The server uses three SQL Server databases:

| Database | Purpose |
|---|---|
| `LYRA_Db` | Main relational app data, identity, companies, touchpoints, policies. |
| `LYRA_CachedDb` | Flattened cached access policies. |
| `LYRA_LogsDb` | Persistent application and verification logs. |

---

## Seeded Development Data

On first startup, the server automatically:

- applies migrations
- creates admin user `admin@lyra` with password `admin`
- creates 30 companies
- creates 30 touchpoints
- creates 30 cyclic access policies
- synchronizes policy cache

Seeded touchpoints use names like:

- `default-tp-01@company-01`
- `default-tp-02@company-02`

Seeded touchpoint secrets use values like:

- `tp-secret-01`
- `tp-secret-02`

Note: the current seed policy path in code is `/api/verify`, while the actual verification endpoint is `/api/verification/verify`. For a working happy-path verification test, create or update a policy to allow `/api/verification/verify`.

---

## Local Development

### Project launch profiles

Default local URLs:

- `http://localhost:5231`
- `https://localhost:7228`

### Docker Compose

The repository includes Docker setup for:

- `lyra.db`
- `lyra.server`

Default compose port mappings:

- SQL Server: `1434 -> 1433`
- LYRA HTTP: `6020 -> 8080`
- LYRA HTTPS: `6021 -> 8081`

The compose file expects an external Docker network:

- `gloria-net`

---

## Configuration

Main configuration areas:

- connection strings
- MILANO client host/API key/timeout
- logging levels

Primary config file:

- `src/LYRA.Server/appsettings.json`

Development overrides:

- `src/LYRA.Server/appsettings.Development.json`

---

## Security Notes

Current codebase behavior:

- payload hash uses SHA-512 over the raw payload string
- timestamp skew window is `+-2 hours`
- replay protection depends on `requestId`
- secrets are stored encrypted at rest in the database

Current implementation caveats worth knowing:

- only HMAC algorithms are implemented
- replay protection is not atomic under concurrency
- seeded admin credentials are development-grade defaults
- current source includes development-style secrets/configuration that should not be used as-is in production

---

## Integration Notes

To make verification succeed:

1. Create an active caller touchpoint.
2. Create an active target touchpoint.
3. Create an enabled access policy from caller to target.
4. Add a rule matching the exact `method` and `path`.
5. Build `bodyHash` from the exact raw payload string.
6. Build the canonical string using `SignatureStringBuilder`.
7. Sign it using the caller secret and matching `SignatureType`.
8. Send `VerifyRequest` to `POST /api/verification/verify`.

---

## Repository Notes

The repository currently does not include a test project.

The README is intended to reflect the current implementation, not the originally planned feature set.

---

## License

MIT License - see [LICENSE](LICENSE).

---

** 🛡 LYRA. Let Yourself Remain Authenticated.**
