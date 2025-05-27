# 🛡️ LYRA — *Let Yourself Remain Authenticated*

**LYRA** is a centralized, policy-based **authorization server** designed to verify **signed requests** between trusted systems — both **internally** and **between companies**.

It introduces the concept of **Trusted Touchpoints** and **Access Policies** to ensure **secure, controlled access** across boundaries.

> ✨ Built to run anywhere. Designed to depend on no one.

---

## 🌐 What is LYRA?

- 🧱 **LYRA.Server** — a self-hosted authorization and verification center for backend systems  
- ⚙️ **LYRA.Client** — a lightweight client for signing outgoing requests (HMAC, RSA, etc.)  
- 🛡️ **AccessPolicy** — rules defining which system can talk to which, and under what conditions  
- 🧼 **Zero External Dependencies** — no third-party libraries; powered only by native .NET 8 APIs

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
      "caller": "bcorp::gateway",
      "target": "acorp::billing",
      "method": "POST",
      "path": "/subscribe",
      "payloadHash": "sha512(...)",
      "signature": "base64(...)"
    }
    ```

2. `acorp`'s **LYRA.Server** receives and validates:
    - Looks up the **Caller Touchpoint**
    - Verifies the **signature**
    - Confirms an `AccessPolicy` exists for the call
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
       ├── OutgoingPolicies → AccessPolicy
       └── IncomingPolicies ← AccessPolicy
```

---

## ✅ Access Policy Example

> "`gateway` of `bcorp` can call `billing` of `acorp` via POST `/subscribe`"

```csharp
new AccessPolicyEntity
{
    CallerId = gateway.Id,
    TargetId = billing.Id,
    Context = AccessContext.Http,
    Operation = "POST /subscribe"
}
```

---

## 💡 Highlights

- ✅ Clean company-to-company and service-to-service trust boundaries  
- ✅ Each touchpoint has a strict role + secret/key + audit  
- ✅ Fast, stateless request verification  
- ✅ Supports `HMAC`, `RSA`, and future key types  
- ✅ Frontend for managing companies, touchpoints, and policies  
- ✅ All names auto-slugified from Display Names  
- ✅ Fully **zero-dependency**: no external packages, no third-party crypto libs, no validation frameworks  
- ✅ Pure .NET 8 — only Microsoft-built building blocks  

---

## 🔧 Tech Stack

- ASP.NET Core 8 + Razor Pages + EF Core  
- SQLite or SQL Server  
- HMAC / RSA verification (System.Security.Cryptography)  
- Docker / local deploy-ready  
- Designed to integrate with **API Gateways** or microservice boundaries  
- Built with **no third-party dependencies**

---

## 📄 License

Licensed under the [MIT License](LICENSE).

**LYRA. She signs. She verifies. She protects.**  
*"LYRA is your internal firewall. Chaos outside. Order inside."*
