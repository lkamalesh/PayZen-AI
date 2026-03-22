# PayZen - Payment System with AI Risk Assistance

`PayZen` is a `.NET 8` payment risk intelligence API built with a clean layered architecture:

- `PaymentAI.API` – ASP.NET Core Web API (controllers, middleware, startup)
- `PaymentAI.Core` – domain entities, DTOs, enums, interfaces
- `PaymentAI.Infrastructure` – EF Core data access, repositories, services, identity seeding, AI integration

It supports merchant onboarding, payment processing with idempotency, configurable risk rules, analyst workflows, audit logging, and AI-based risk explanations via Azure AI Foundry.

---

## Features

- Merchant registration and login with JWT
- Role-based authorization (`Merchant`, `Analyst`)
- Payment processing with:
  - idempotency key support
  - API key validation middleware for `/api/payment`
  - risk scoring
- Dynamic risk rule CRUD
- Transaction monitoring APIs
- Audit logging
- AI explanation endpoint for flagged/high-risk transactions
- RAG pipeline using internal compliance / AML guideline documents stored in Azure Blob Storage
- SQL Server persistence with EF Core + ASP.NET Identity

---

## Tech Stack

- ASP.NET Core Web API (`net8.0`)
- Entity Framework Core (`SQL Server`)
- ASP.NET Core Identity
- JWT Bearer Authentication
- AutoMapper
- Serilog (file logging)
- Azure OpenAI & Azure AI Search
- Azure AI Projects / Foundry agent integration
- Azure App Service deployment (with Managed Identity for Azure resource access)

---

## RAG + AI Agent Implementation

This project includes a Retrieval-Augmented Generation (RAG) workflow for risk explanation:

- Internal compliance documents and AML guidelines are stored in Azure Blob Storage.
- Document chunks are indexed in Azure AI Search for retrieval.
- Azure OpenAI is used to generate contextual risk explanations.
- A Microsoft Foundry AI Agent orchestrates retrieval + response generation for analyst-facing AI explanations.

---

## Solution Structure

```text
PaymentAI.sln
├─ PaymentAI.API/
│  ├─ Controllers/
│  ├─ Middlewares/
│  ├─ Properties/
│  ├─ Program.cs
│  └─ appsettings*.json
├─ PaymentAI.Core/
│  ├─ DTOs/
│  ├─ Entities/
│  ├─ Enums/
│  └─ Interfaces/
└─ PaymentAI.Infrastructure/
   ├─ Configuration/
   ├─ Data/
   ├─ Mappings/
   ├─ Repositories/
   └─ Services/
```

---

## Prerequisites

- .NET SDK 8.0+
- SQL Server (or SQL Server Express / LocalDB)
- Azure account + Azure AI Foundry project (for AI explanation feature)

---

## Configuration

Set values in `PaymentAI.API/appsettings.Development.json` (or App Service settings using `__` naming):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "<sql-connection-string>"
  },
  "JwtSettings": {
    "Issuer": "<issuer>",
    "Audience": "<audience>",
    "Key": "<strong-secret-key>"
  },
  "Credentials": {
    "Analyst": {
      "Email": "analyst@yourdomain.com",
      "Password": "<strong-password>"
    }
  },
  "OpenAISettings": {
    "Endpoint": "https://<resource>.services.ai.azure.com/api/projects/<project>",
    "ApiKey": "<if-used>",
    "AgentName": "<agent-name>",
    "DeploymentName": "<model-deployment>"
  }
}
```

### Important

- `OpenAISettings.Endpoint` must be the **Foundry project endpoint** (`.../api/projects/<project>`).
- In Azure App Service, `DefaultAzureCredential` uses Managed Identity. Grant that identity access to Foundry/OpenAI resources.

---

## Running Locally

## Hosted Frontend

- `https://payzen-ai.vercel.app`

---

## Git Download / Clone

Clone the repository using Git:

```bash
git clone https://github.com/lkamalesh/PayZen-AI.git
cd PaymentAI
```

Or use GitHub UI: click `Code` → `Download ZIP`, then extract and open the solution.

---

From solution root:

```bash
dotnet restore
dotnet build
dotnet run --project PaymentAI.API
```

Swagger UI:

- `https://localhost:7114/swagger`
- `http://localhost:5100/swagger`

---

## Authentication & Authorization

### JWT

Login returns:

```json
{ "Token": "<jwt>" }
```

Include in protected requests:

```http
Authorization: Bearer <jwt>
```

JWT includes claims:

- `name`
- `email`
- `MerchantId`
- `ApiKey`
- role claims (`ClaimTypes.Role`)

### Roles

- `Analyst` required for:
  - `/api/Rule/*`
  - `/api/Transaction/*`
  - `/api/Audit/*`
  - `/api/Merchant/*`
  - `/api/AI/*`

---

## API Key Middleware

`ApiKeyMiddleware` validates `x-api-key` for `/api/payment` routes.

Required header:

```http
x-api-key: <merchant-api-key>
```

If missing/invalid, API returns `401`/`403`.

---

## Core Endpoints

### Auth

- `POST /api/Auth/Register`
- `POST /api/Auth/Login`

### Payment

- `POST /api/Payment`
  - Requires `Idempotency-Key` header
  - Requires `x-api-key` header (middleware)

### Rules (Analyst)

- `GET /api/Rule/GetAll`
- `POST /api/Rule/Create`
- `PUT /api/Rule/Update`
- `DELETE /api/Rule/Delete`

### Transactions (Analyst)

- `GET /api/Transaction/GetAll`
- `GET /api/Transaction/{merchantId}`
- `GET /api/Transaction/GetRecentTransactions/{customerId}/{limit}`
- `GET /api/Transaction/status/{status}`

### Audit (Analyst)

- `GET /api/Audit/GetAll`

### Merchant (Analyst)

- `GET /api/Merchant/{merchantId}`
- `GET /api/Merchant/GetAll`

### AI (Analyst)

- `GET /api/AI/{transactionId}`

---

## Sample Request Payloads

### Register Merchant

```json
{
  "userName": "fastpay",
  "country": "US",
  "email": "fastpay@example.com",
  "password": "StrongPass@123"
}
```

### Login

```json
{
  "email": "fastpay@example.com",
  "password": "StrongPass@123"
}
```

### Create Payment

Headers:

```http
Idempotency-Key: 7f1e43d1-9759-4318-9ac8-5d59d2e5f677
x-api-key: <merchant-api-key>
```

Body:

```json
{
  "customerId": "CUST-1001",
  "apiKey": "<merchant-api-key>",
  "amount": 1250.50,
  "country": "US",
  "currency": "USD",
  "paymentMethod": "CreditCard"
}
```

Allowed `paymentMethod` values:

- `CreditCard`
- `DebitCard`
- `BankTransfer`
- `PayPal`
- `Crypto`
- `WireTransfer`

---

## Database

`AppDbContext` contains:

- `Transactions`
- `Customers`
- `RiskRules`
- `AuditLogs`
- ASP.NET Identity tables

Includes:

- relationships and delete behaviors
- unique index on `Transaction.IdempotencyKey`
- enum-to-string conversion for transaction `Status` and `PaymentMethod`

---

## Deployment Notes (Azure App Service)

- Enable system-assigned Managed Identity
- Assign proper roles on Foundry/OpenAI/Search as needed
- Configure App Settings with double underscore keys (e.g. `JwtSettings__Key`)
- Ensure CORS origin includes your frontend domain (`Program.cs` currently allows `https://payzen-ai.vercel.app`)

---

## Logging

Serilog writes rolling logs to:

- `logs/log-.txt`

---

## Known Operational Notes

- `UseExceptionHandler("/error")` is configured for non-development. Add an `/error` endpoint if you want clean centralized error responses.
- Keep secrets out of source control; use environment variables/App Service settings.

---

## Contributing

1. Create a feature branch
2. Commit focused changes
3. Run `dotnet build`
4. Open a pull request

---

## License

Add your preferred license (`MIT`, `Apache-2.0`, etc.) in a `LICENSE` file.

---

Developed by: Kamalesh Loganathan
