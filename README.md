# 🛡️ LYRA — *Let Yourself Remain Authenticated*

**LYRA** is a centralized, policy-based **authorization server** designed to verify **signed requests** between trusted systems — both **internally** and **between companies**.

It introduces the concept of **Trusted Touchpoints** and **Access Policies** to ensure **secure, controlled access** across boundaries.

> ✨ Built to run anywhere. Designed to depend on no one.

---

## 🌐 What is LYRA?

- 🧱 **LYRA.Server** — a self-hosted authorization and verification center for backend systems  
- ⚙️ **[LYRA.Client](https://github.com/pasternak2048/yuriipasternak-lyra-client)** — a lightweight client for signing outgoing requests (HMAC, RSA, etc.)  
- 🔐 **[LYRA.Security](https://github.com/pasternak2048/yuriipasternak-lyra-security)** — shared crypto, signature, and contract foundation used by **LYRA.Server** and **LYRA.Client**
- 🛡️ **AccessPolicy** — rules defining which system can talk to which, and under what conditions  
- 🧼 **Zero Dependencies** — built on .NET 8 with **only official Microsoft libraries** (EF Core, SQL Server, System.Security)
- 🚀 **High-Performance Caching** — uses denormalized cache DB + EF Core interceptor to synchronize changes in real-time without recomputing all policies
- 🧠 **Memory Cache Layer** — recent policies are stored in memory for ultra-fast access, reducing DB queries to near-zero

---

## 🧠 Core Concepts

| Concept             | Description |
|--------------------|-------------|
| `Company`           | A tenant or organization managing multiple systems |
| `TrustedTouchpoint` | A service or system component acting as a **Caller**, **Target**, or **Both** |
| `TouchpointMode`    | Defines direction: `CallerOnly`, `TargetOnly`, or `Both` |
| `AccessPolicy`      | A rule that grants a caller the right to access a specific target |
| `AccessContext`     | Type of interaction: `Http`, `Event`, `Cache`, `Grpc`, `Internal`, or `Soap` |
| `SignatureType`     | Signature algorithm: `HMAC`, `RSA`, or `None` |

---

## 🔁 Authorization Flow

1. A service from `bcorp` sends a signed request:
    ```json
    {
      "caller": "gateway@bcorp",
      "target": "billing@acorp",
      "method": "POST",
      "path": "/subscribe",
      "payload": " ... ",
      "payloadHash": "sha512(...)",
      "timestamp": "2025-05-31T12:00:00Z",
      "context": "Http",
      "signature": "base64(...)"
    }
    ```

2. `acorp`'s **LYRA.Server** receives and validates:
    - Looks up the **Caller Touchpoint**
    - Verifies the **signature**
    - Confirms an `AccessPolicy` exists for the call (using SystemName)
    - Approves (200) or denies (403) the request

---

## 🧱 Entity Structure

```csharp
Company
 └── TrustedTouchpoints
       ├── Mode: CallerOnly / TargetOnly / Both
       ├── Secret
       ├── UseCompanySecret
       ├── SignatureType
       ├── AllowedSourceIp
       ├── OutgoingPolicies → AccessPolicy (CallerId + CallerSystemName)
       └── IncomingPolicies ← AccessPolicy (TargetId + TargetSystemName)
```

---

## ✅ Access Policy Example

> "`gateway` of `bcorp` can call `billing` of `acorp` via POST `/subscribe`"

```csharp
new AccessPolicyEntity
{
    CallerSystemName = "gateway@bcorp",
    TargetSystemName = "billing@acorp",
    Context = AccessContext.Http,
    Operation = "POST /subscribe"
}
```

---

## ⚡ Smart Caching System

LYRA automatically mirrors validated policies into a **denormalized cache table** (CachedAccessPolicyEntity).  
This cache:
- Is **auto-updated** on every change via a custom `SaveChangesInterceptor`
- Reacts not only to policies, but also to changes in:
  - `Company` (e.g., secret rotation or disabled state)
  - `Touchpoint` (e.g., IP restrictions, mode updates)
- Is fast to query, with **compound keys** for fast lookups
- Fully rebuildable via `ReplaceAllAsync()` if needed

In addition, an **in-memory caching layer** (`MemoryCache`) stores recently used policies for instant retrieval.  
This ensures that frequent validations complete in **microseconds**, with **zero DB calls** in the hot path.

This architecture allows **constant-time access policy validation**, even under load.

---

## 📈 Real-Time Log Monitoring

LYRA includes a **Live Log Monitor**, powered by **SignalR** and optimized for security observability.

- 📊 Real-time streaming of log entries to the browser via SignalR
- 📋 Logs include timestamp, status, type, caller, target, hash, and description
- 🖥️ Displayed in a responsive console-style or tabular interface
- 🎨 Status-based color coding (Success / Fail / Critical / Warning / Info)
- 🧾 Logs are structured, stored in SQL Server, and streamed via SignalR

> This lets you **instantly observe validation behavior** as it happens — no page refresh required.

---

## 💡 Highlights

- ✅ Clean company-to-company and service-to-service trust boundaries  
- ✅ Each touchpoint has a strict role + secret/key + audit  
- ✅ Denormalized names (CallerSystemName + TargetSystemName) for fast lookup  
- ✅ High-performance policy cache with EF interceptor  
- ✅ Fast memory-layer cache for runtime reads  
- ✅ Supports `HMAC`, `RSA`, and future key types  
- ✅ Frontend for managing companies, touchpoints, and policies  
- ✅ All names auto-slugified from Display Names  
- ✅ **Zero third-party dependencies** — no external crypto libs, no validation frameworks  
- ✅ Built on **.NET 8 + EF Core + SQL Server** — using **official Microsoft libraries only**  
- ✅ Modular architecture: LYRA.Server uses LYRA.Security for core crypto/signature logic

---

## 🔧 Tech Stack

- ASP.NET Core 8 + Razor Pages + EF Core  
- Microsoft SQL Server  
- Microsoft.Extensions.Caching.Memory  
- System.Security.Cryptography (HMAC / RSA)  
- LYRA.Security library (shared signature + crypto logic)  
- SignalR
- Docker / local deploy-ready  
- Built with **no third-party dependencies**

---

## 📄 License

Licensed under the [MIT License](LICENSE).

**LYRA. She signs. She verifies. She protects.**  
*"LYRA is your internal firewall. Chaos outside. Order inside."*
