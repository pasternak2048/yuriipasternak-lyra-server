# 🛡️ LYRA — *Let Yourself Remain Authenticated*

**LYRA** is a centralized, policy-based **authorization server** designed to verify **signed requests** between trusted systems — both **internally** and **between companies**.

It introduces the concept of **Trusted Touchpoints** and **Access Policies** to ensure **secure, controlled access** across boundaries.

> ✨ Built to run anywhere. Designed to depend on no one.

---

## 🌐 What is LYRA?

- 🧱 **LYRA.Server** — a self-hosted authorization and verification center for backend systems  
- ⚙️ **LYRA.Client** — a lightweight client for signing outgoing requests (HMAC, RSA, etc.)  
- 🔐 **LYRA.Security** — shared crypto, signature, and contract foundation used by both client and server  
- 🛡️ **AccessPolicy** — rules defining which system can talk to which, and under what conditions  
- 🧼 **Zero Dependencies** — built on .NET 8 with **only official Microsoft libraries** (EF Core, SQL Server, System.Security)

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

## 💡 Highlights

- ✅ Clean company-to-company and service-to-service trust boundaries  
- ✅ Each touchpoint has a strict role + secret/key + audit  
- ✅ Denormalized names (CallerSystemName + TargetSystemName) for fast lookup  
- ✅ Fast, stateless request verification  
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
- System.Security.Cryptography (HMAC / RSA)  
- LYRA.Security library (shared signature + crypto logic)  
- Docker / local deploy-ready  
- Designed to integrate with **API Gateways** or microservice boundaries  
- Built with **no third-party dependencies**

---

## 📄 License

Licensed under the [MIT License](LICENSE).

**LYRA. She signs. She verifies. She protects.**  
*"LYRA is your internal firewall. Chaos outside. Order inside."*
