# 🛡️ LYRA — *Let Yourself Remain Authenticated*

**LYRA** is a lightweight, policy-driven platform for signed, trusted communication between services.

This repository contains the **LYRA.Server** — a centralized service that issues and verifies cryptographic request signatures, enforcing access policies between microservices.

---

## 🌐 What is LYRA?

LYRA verifies the authenticity of every request.

It issues cryptographic signatures (`/sign`) and validates them (`/verify`),  
enforcing service-to-service access policies with speed, elegance, and minimal configuration.

---

## 🔧 Features

- 🔐 HMAC-based request signing (SHA-512)
- 📜 Centralized access policy enforcement
- 🛡 `/sign` and `/verify` endpoints
- ⚙️ JSON-based configuration (no DB required)
- 🧪 Optional bypass mode for development
- 🎛 Optional Blazor admin panel (coming soon)

---

## 🔍 Example: Verify Signature

```http
POST /verify
Content-Type: application/json

{
  "serviceName": "catalog",
  "unixTime": 1716400000,
  "payloadHash": "sha512(...payload...)",
  "signature": "...",
  "targetService": "subscription",
  "method": "POST",
  "path": "/subscribe"
}
```
## 📄 License

This project is licensed under the [MIT License](LICENSE).

LYRA. She signs. She verifies. She protects.
