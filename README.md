# Hospital Management SaaS Platform

A healthcare operations application built with **Angular 19**, **.NET 10**, Entity Framework Core, and Microsoft SQL Server.

## Features

- Patient registration and identity-document input with country-specific formatting
- Doctor and workforce administration, specialties, and consultation fees
- Patient-doctor appointment scheduling
- Inpatient admissions, beds, rooms, wards, and occupancy tracking
- Clinical encounters, diagnosis coding, examinations, and prescriptions
- Laboratory and radiology orders and result entry
- Pharmacy inventory, suppliers, and procurement workflows
- Billing, invoices, payments, and insurance information
- Role-based access for administrative and clinical staff
- Multi-tenant data access patterns

## Technology stack

| Layer | Technology |
| --- | --- |
| Frontend | Angular 19, TypeScript, Signals, Standalone Components |
| Styling | SCSS |
| Backend | .NET 10, ASP.NET Core Web API |
| Data access | Entity Framework Core |
| Database | Microsoft SQL Server |
| Security | JWT bearer authentication and tenant-aware query filtering |

## Repository structure

```text
.
├── backend/
│   └── src/
│       └── HospitalSaaS.Api/
└── frontend/
```

## Prerequisites

- .NET 10 SDK
- Node.js version supported by Angular 19
- npm
- SQL Server, LocalDB, or SQL Server Express

## Run locally

### 1. Configure the database

Review the backend configuration and set a local SQL Server connection string using development-only settings or environment variables. Do not commit real credentials.

### 2. Apply database migrations and start the API

```bash
cd backend/src/HospitalSaaS.Api
dotnet ef database update
dotnet run
```

The API is documented by the project as using https://localhost:7001 or http://localhost:5000. Confirm the actual bound address in the application startup output.

### 3. Install and start the frontend

```bash
cd frontend
npm install
npm start
```

Open http://localhost:4200/ in your browser.

## Demo accounts and credential safety

Earlier versions of this README published example email addresses and a shared default password for several seeded roles. Those passwords have been removed from public documentation.

For local evaluation, inspect the development-only seed configuration and create unique local credentials. Do not reuse sample passwords on a deployed system. Before any public deployment, disable or replace predictable seeded accounts, rotate any credentials that may have been used outside a local environment, and provision users through a secure administrator workflow.

**Healthcare data warning:** Use synthetic test data only in development and demonstrations. Do not commit patient information, medical records, access tokens, connection strings, or other sensitive data.

## License

The previous README identified the project as MIT-licensed. Verify that a matching `LICENSE` file exists in the repository before redistributing the project under that license.
