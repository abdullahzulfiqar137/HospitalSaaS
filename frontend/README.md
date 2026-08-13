# Hospital SaaS Frontend

Angular 19 standalone frontend for the .NET 10 Hospital SaaS API. The projects remain independent and communicate only over HTTP.

## Run

```powershell
npm.cmd install
npm.cmd start
```

Development uses `https://localhost:7185/api/v1`, matching the backend HTTPS launch profile. Start the API separately from `../backend` and trust its development certificate. Production uses the same-origin `/api/v1`; configure the reverse proxy or replace `environment.prod.ts` for the deployed API origin.

Login requires the tenant GUID because the backend resolves anonymous authentication requests from `X-Tenant-ID`. Once authenticated, the signed JWT `tenant_id` claim is authoritative. Tokens are session-scoped, attached by the auth interceptor, and refreshed through the backend refresh-token endpoint.

## API integration map

| Frontend feature/service                 | Backend controller             | Routes used                                                                                                                |
| ---------------------------------------- | ------------------------------ | -------------------------------------------------------------------------------------------------------------------------- |
| Authentication / `AuthService`           | `AuthController`               | `POST auth/login`, `POST auth/refresh`, `POST auth/logout`, `POST auth/revoke`, `POST auth/change-password`, `GET auth/me` |
| Dashboard / `DashboardService`           | `DashboardController`          | `GET dashboard`                                                                                                            |
| Patients / `PatientService`              | `PatientsController`           | `GET/POST patients`, `GET/PUT/DELETE patients/{id}`                                                                        |
| Doctors / `DoctorService`                | `DoctorsController`            | `GET/POST doctors`, `GET/PUT/DELETE doctors/{id}`, `POST doctors/{id}/activate                                             | deactivate` |
| Departments / `DepartmentService`        | `DepartmentsController`        | `GET/POST departments`, `GET/PUT/DELETE departments/{id}`                                                                  |
| Appointments / `AppointmentService`      | `AppointmentsController`       | `GET/POST appointments`, `GET/PUT appointments/{id}`, `POST appointments/{id}/actions`                                     |
| Admissions / `AdmissionService`          | `AdmissionsController`         | `POST admissions`, `GET admissions/{id}`, `POST admissions/wards                                                           | rooms       | beds`, transfer, history, discharge |
| Medical records / `MedicalRecordService` | `MedicalRecordsController`     | `GET/POST medical-records`, `GET/DELETE medical-records/{id}`, diagnoses sub-resource                                      |
| Prescriptions / `PrescriptionService`    | `PrescriptionsController`      | `GET/POST prescriptions`, `GET prescriptions/{id}`                                                                         |
| Pharmacy / `PharmacyService`             | `PharmacyController`           | medicine collection/detail and stock sub-resources under `pharmacy`                                                        |
| Laboratory / `LaboratoryService`         | `LaboratoryController`         | tests, orders, actions, results, and history under `laboratory`                                                            |
| Radiology / `RadiologyService`           | `RadiologyController`          | tests, orders, reports, and history under `radiology`                                                                      |
| Billing / `BillingService`               | `BillingController`            | invoices and invoice payments under `billing`                                                                              |
| Payments / `PaymentService`              | `BillingController`            | `GET billing/payments`, refund action                                                                                      |
| Inventory / `InventoryService`           | `InventoryController`          | inventory items, stock adjustments, purchase orders and actions                                                            |
| Suppliers / `SupplierService`            | `SuppliersController`          | `GET/POST suppliers`, `GET/DELETE suppliers/{id}`                                                                          |
| Insurance                                | `InsuranceController`          | `GET/POST insurance`                                                                                                       |
| Staff / `StaffService`                   | `StaffController`              | `GET/POST staff`                                                                                                           |
| Users / `UserService`                    | `UsersController`              | `GET/POST users`                                                                                                           |
| Roles and permissions                    | `RolesController` / JWT claims | `GET/POST roles`; permissions are claim values, not a separate controller                                                  |
| Tenants / `TenantService`                | `TenantsController`            | `POST tenants`                                                                                                             |
| Documents / `DocumentService`            | `DocumentsController`          | `GET/POST documents`, `DELETE documents/{id}`                                                                              |
| Notifications / `NotificationApiService` | `NotificationsController`      | `GET/POST notifications`                                                                                                   |
| Subscriptions / `SubscriptionService`    | `SubscriptionsController`      | `GET/POST subscriptions`                                                                                                   |
| Audit logs                               | `AuditLogsController`          | `GET audit-logs`                                                                                                           |

Paged endpoints return `{ items, pageNumber, pageSize, totalCount, totalPages, hasPreviousPage, hasNextPage }`. API errors return `{ code, message, correlationId, errors? }` and are handled globally.

## Verification

```powershell
npm.cmd run build
npm.cmd test -- --watch=false
npm.cmd run e2e
```

Playwright runs anonymous-route and login-validation checks in installed Chrome. The live backend
login scenario runs when `E2E_TENANT_ID`, `E2E_ADMIN_EMAIL`, and `E2E_ADMIN_PASSWORD` are set in the
environment; otherwise only that scenario is skipped.

The project includes a Windows-safe headless Chrome launcher in `karma.conf.js`. The patient workflow is the full CRUD vertical slice: guarded lazy routes, backend-compatible `query` search and pagination, create/update forms matching backend contracts, deletion, loading/error/empty states, and success notifications.

The local integration verification completed against SQL Server using the seeded tenant: login returned a JWT, an authenticated patient create returned `PAT-2026-000001`, and fetching the new record by ID matched the stored response.
