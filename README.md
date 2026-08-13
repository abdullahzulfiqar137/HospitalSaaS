# 🏥 Hospital Management SaaS Platform

> **A Modern, Multi-Tenant Healthcare Operations Console & Medical SaaS Workspace**

Built with **Angular 19** (Signals & Standalone Architecture) and **.NET 10 Web API** with Entity Framework Core & SQL Server.

---

## 🌟 Key Features

- 🏥 **Patient Management & International Identity Validation**:
  - Live CNIC / Document ID masking with dashes (`-`) and country-specific `maxlength` limits.
  - HD Flag selectors for Pakistan, USA, UAE, India, Malaysia, Brazil, Saudi Arabia, Turkey, Egypt, Indonesia.
  - Dynamic identity example placeholders based on selected country.

- 🩺 **Doctor & Workforce Administration**:
  - Specialty assignment, consultation fee scheduling, and license governance.
  - Interactive doctor registration modals.

- 📅 **Appointments & Scheduling**:
  - Direct patient-doctor booking workflows across Cardiology, Neurology, Orthopedics, Pediatrics, Emergency, and General Practice.

- 🛌 **Inpatient Admissions & Ward Operations**:
  - Bed, room, and ward management (General Ward, ICU, CCU) with real-time occupancy tracking.

- 📋 **Clinical encounters & Prescriptions**:
  - Chief complaints, ICD-10 diagnostic coding, physical examinations, and digital prescriptions.

- 🧪 **Diagnostics, Laboratory & Radiology**:
  - Lab test catalog (CBC, Lipid Panel, Blood Glucose), radiology imaging orders (X-Ray, MRI, CT Scan), and result entry.

- 💊 **Pharmacy & Medical Inventory Procurement**:
  - Medicine stock management, reorder level tracking, supplier catalog, and purchase order workflow.

- 💳 **Billing, Invoicing & Financial Governance**:
  - Automated invoice generation, partial/full payment recording, and insurance policy coverage.

- 🔐 **Role-Based Governance & Executive Security**:
  - 1-Click Role Presets (Admin, Doctor, Nurse, Lab Tech, Pharmacist, Billing Clerk, Receptionist).
  - 9 Categorized Permission Matrix groups with fine-grained policy toggles.
  - Executive Glassmorphic User Profile menu & sticky navigation.

---

## 🛠️ Technology Stack

| Layer | Technology |
| :--- | :--- |
| **Frontend Framework** | Angular 19 (Standalone Components, Signals, Reactive Forms) |
| **Styling** | Vanilla SCSS, Glassmorphism Design System, Plus Jakarta Sans & Outfit Fonts |
| **Backend API** | .NET 10 Web API (C#) |
| **ORM / Database** | Entity Framework Core 10, SQL Server (`HospitalSaaSDb`) |
| **Security** | JWT Bearer Authentication, Multi-Tenant Query Filtering |

---

## 🚀 Getting Started

### 1. Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Node.js v18+](https://nodejs.org/)
- SQL Server (LocalDB or Express / Enterprise)

### 2. Backend Setup
```bash
cd backend/src/HospitalSaaS.Api
dotnet ef database update
dotnet run
```
> The API server will start on `https://localhost:7001` or `http://localhost:5000`.

### 3. Frontend Setup
```bash
cd frontend
npm install
npm start
```
> Navigate to `http://localhost:4200/` in your browser.

---

## 🔐 System Logins (Default Seed Data)

| Role | Email | Password |
| :--- | :--- | :--- |
| **Hospital Administrator** | `admin@hospital.com` | `Password123!` |
| **Doctor** | `doctor@hospital.com` | `Password123!` |
| **Lab Technician** | `labtech@hospital.com` | `Password123!` |
| **Pharmacist** | `pharmacist@hospital.com` | `Password123!` |
| **Patient** | `patient@hospital.com` | `Password123!` |

---

## 📄 License
This project is open source and available under the [MIT License](LICENSE).
