# 🛡️ LYRA — *Let Yourself Remain Authenticated*

**LYRA** is a lightweight, policy-driven **authorization platform**  
for verifying **signed incoming service-to-service requests** between companies or internal systems.

LYRA provides a centralized mechanism to verify trust between components  
through the concepts of **TrustedAgent** and **AccessPolicy**.

---

## 🌐 What is LYRA?

- 🧱 **LYRA.Server** — the authorization server deployed by the target company
- ⚙️ **LYRA.Client** — a client library to sign outgoing requests
- 🛡️ **AccessPolicy** — a rule that allows one agent to access another under defined conditions

---

## 🧠 Core Concepts

| Concept           | Description |
|------------------|-------------|
| `Company`         | An organization that owns one or more agents |
| `TrustedAgent`    | A logical unit (e.g. service/module) that can act as a **Caller** or **Target** |
| `AgentMode`       | Defines agent's role: `CallerOnly`, `TargetOnly`, or `Both` |
| `AccessPolicy`    | A rule that determines whether a caller agent is allowed to access a target agent for a specific route and method |

---

## 🔁 Authorization Flow

1. Agent B (`gateway@bcorp`) signs and sends a request:
    ```json
    {
      "caller": "bcorp::gateway",
      "target": "acorp::billing",
      "method": "POST",
      "path": "/subscribe",
      "payloadHash": "sha512(...)",
      "signature": "..."
    }
    ```

2. LYRA.Server (hosted by `acorp`) receives and processes the request:
    - Looks up the `CallerAgent`
    - Verifies signature using its secret
    - Confirms existence of a valid policy allowing access to `billing@acorp`
    - Approves (200) or denies (403) the request

---

## 📦 Entity Structure

```csharp
Company
 └── TrustedAgents
       ├── Mode: CallerOnly / TargetOnly / Both
       ├── Secret (for signing)
       ├── OutgoingPolicies → AccessPolicy
       └── IncomingPolicies ← AccessPolicy
```

---

## ✅ Access Policy Example

> "Agent `gateway` of `bcorp` is allowed to call `billing` of `acorp` via POST `/subscribe`"

```csharp
new AccessPolicyEntity
{
    CallerAgentId = gateway.Id,
    TargetAgentId = billing.Id,
    Method = "POST",
    PathPattern = "/subscribe"
}
```

---

## 💡 Highlights

- ✅ Supports flexible route matching (`/invoice/*`, `/v1/payment/{id}`)
- ✅ AgentMode ensures strict role separation
- ✅ One-way trust — A → B ≠ B → A
- ✅ Agent-level or Company-level secret usage
- ✅ Agents unique within a company
- ✅ Fast runtime signature verification

---

## 🔧 Tech Stack

- ASP.NET Core + EF Core
- SQLite or SQL Server
- Optional JWT integration
- Deployable locally, via Docker, or as part of a Gateway

---

## 🚀 Roadmap

- 🔐 Add support for `RSA` / `ECDSA` keys
- 📊 Admin dashboard for visualizing connections
- 🧩 Policy DSL integration
- 📘 Auto-generated integration docs for partners

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

LYRA. She signs. She verifies. She protects.
---
*"LYRA is your internal firewall. Chaos outside. Order inside."*
