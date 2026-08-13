using FluentValidation;
using HospitalSaaS.Application.Administration;
using HospitalSaaS.Application.Admissions;
using HospitalSaaS.Application.Appointments.Commands.BookAppointment;
using HospitalSaaS.Application.Appointments.Commands.ChangeAppointmentStatus;
using HospitalSaaS.Application.Appointments.Commands.UpdateAppointment;
using HospitalSaaS.Application.Appointments.Models;
using HospitalSaaS.Application.Appointments.Queries.GetAppointmentById;
using HospitalSaaS.Application.Appointments.Queries.SearchAppointments;
using HospitalSaaS.Application.AuditLogs;
using HospitalSaaS.Application.Authentication;
using HospitalSaaS.Application.Authentication.Models;
using HospitalSaaS.Application.Billing;
using HospitalSaaS.Application.Clinical;
using HospitalSaaS.Application.Common.Behaviors;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Dashboard;
using HospitalSaaS.Application.Departments;
using HospitalSaaS.Application.Doctors.Commands.CreateDoctor;
using HospitalSaaS.Application.Doctors.Commands.DeleteDoctor;
using HospitalSaaS.Application.Doctors.Commands.SetDoctorStatus;
using HospitalSaaS.Application.Doctors.Commands.UpdateDoctor;
using HospitalSaaS.Application.Doctors.Models;
using HospitalSaaS.Application.Doctors.Queries.GetDoctorById;
using HospitalSaaS.Application.Doctors.Queries.SearchDoctors;
using HospitalSaaS.Application.Documents;
using HospitalSaaS.Application.Insurance;
using HospitalSaaS.Application.Inventory;
using HospitalSaaS.Application.Laboratory;
using HospitalSaaS.Application.Notifications;
using HospitalSaaS.Application.Patients.Commands.CreatePatient;
using HospitalSaaS.Application.Patients.Commands.DeletePatient;
using HospitalSaaS.Application.Patients.Commands.UpdatePatient;
using HospitalSaaS.Application.Patients.Models;
using HospitalSaaS.Application.Patients.Queries.GetPatientById;
using HospitalSaaS.Application.Patients.Queries.SearchPatients;
using HospitalSaaS.Application.Pharmacy;
using HospitalSaaS.Application.Radiology;
using HospitalSaaS.Application.Staff;
using HospitalSaaS.Application.Subscriptions;
using HospitalSaaS.Application.Suppliers;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalSaaS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreatePatientCommandValidator>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        services.AddScoped<IRequestDispatcher, RequestDispatcher>();
        services.AddScoped<
            IRequestHandler<CreatePatientCommand, Result<PatientDto>>,
            CreatePatientCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<UpdatePatientCommand, Result<PatientDto>>,
            UpdatePatientCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<DeletePatientCommand, Result>,
            DeletePatientCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>,
            GetPatientByIdQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchPatientsQuery, PagedResult<PatientDto>>,
            SearchPatientsQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateDoctorCommand, Result<DoctorDto>>,
            CreateDoctorCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<UpdateDoctorCommand, Result<DoctorDto>>,
            UpdateDoctorCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<SetDoctorStatusCommand, Result>,
            SetDoctorStatusCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<DeleteDoctorCommand, Result>,
            DeleteDoctorCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetDoctorByIdQuery, Result<DoctorDto>>,
            GetDoctorByIdQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchDoctorsQuery, PagedResult<DoctorDto>>,
            SearchDoctorsQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<BookAppointmentCommand, Result<AppointmentDto>>,
            BookAppointmentCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<ChangeAppointmentStatusCommand, Result<AppointmentDto>>,
            ChangeAppointmentStatusCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetAppointmentByIdQuery, Result<AppointmentDto>>,
            GetAppointmentByIdQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchAppointmentsQuery, PagedResult<AppointmentDto>>,
            SearchAppointmentsQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateDepartmentCommand, Result<DepartmentDto>>,
            CreateDepartmentCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<UpdateDepartmentCommand, Result<DepartmentDto>>,
            UpdateDepartmentCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<DeleteDepartmentCommand, Result>,
            DeleteDepartmentCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetDepartmentByIdQuery, Result<DepartmentDto>>,
            GetDepartmentByIdQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchDepartmentsQuery, PagedResult<DepartmentDto>>,
            SearchDepartmentsQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateWardCommand, Result<WardDto>>,
            CreateWardCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateRoomCommand, Result<RoomDto>>,
            CreateRoomCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateBedCommand, Result<BedDto>>,
            CreateBedCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<AdmitPatientCommand, Result<AdmissionDto>>,
            AdmitPatientCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<TransferAdmissionCommand, Result<AdmissionDto>>,
            TransferAdmissionCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<DischargeAdmissionCommand, Result>,
            DischargeAdmissionCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetAdmissionByIdQuery, Result<AdmissionDto>>,
            GetAdmissionByIdQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateMedicalRecordCommand, Result<MedicalRecordDto>>,
            CreateMedicalRecordCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetMedicalRecordQuery, Result<MedicalRecordDto>>,
            GetMedicalRecordQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchMedicalRecordsQuery, PagedResult<MedicalRecordDto>>,
            SearchMedicalRecordsQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<DeleteMedicalRecordCommand, Result>,
            DeleteMedicalRecordCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<CreatePrescriptionCommand, Result<PrescriptionDto>>,
            CreatePrescriptionCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetPrescriptionQuery, Result<PrescriptionDto>>,
            GetPrescriptionQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchPrescriptionsQuery, PagedResult<PrescriptionDto>>,
            SearchPrescriptionsQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateRadiologyOrderCommand, Result<RadiologyOrderDto>>,
            CreateRadiologyOrderCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<RecordRadiologyReportCommand, Result<RadiologyOrderDto>>,
            RecordRadiologyReportCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetRadiologyOrderQuery, Result<RadiologyOrderDto>>,
            GetRadiologyOrderQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchRadiologyOrdersQuery, PagedResult<RadiologyOrderDto>>,
            SearchRadiologyOrdersQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateMedicineCommand, Result<MedicineDto>>,
            CreateMedicineCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetMedicineQuery, Result<MedicineDto>>,
            GetMedicineQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchMedicinesQuery, PagedResult<MedicineDto>>,
            SearchMedicinesQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<DeleteMedicineCommand, Result>,
            DeleteMedicineCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<RecordMedicineStockCommand, Result<StockTransactionDto>>,
            RecordMedicineStockCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetMedicineStockQuery, Result<int>>,
            GetMedicineStockQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateLabOrderCommand, Result<LabOrderDto>>,
            CreateLabOrderCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<RecordLabResultCommand, Result<LabOrderDto>>,
            RecordLabResultCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetLabOrderQuery, Result<LabOrderDto>>,
            GetLabOrderQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchLabOrdersQuery, PagedResult<LabOrderDto>>,
            SearchLabOrdersQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateInvoiceCommand, Result<InvoiceDto>>,
            CreateInvoiceCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetInvoiceQuery, Result<InvoiceDto>>,
            GetInvoiceQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchInvoicesQuery, PagedResult<InvoiceDto>>,
            SearchInvoicesQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<RecordPaymentCommand, Result<PaymentDto>>,
            RecordPaymentCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<RefundPaymentCommand, Result<PaymentDto>>,
            RefundPaymentCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchPaymentsQuery, PagedResult<PaymentDto>>,
            SearchPaymentsQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateInventoryItemCommand, Result<InventoryItemDto>>,
            CreateInventoryItemCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetInventoryItemQuery, Result<InventoryItemDto>>,
            GetInventoryItemQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchInventoryItemsQuery, PagedResult<InventoryItemDto>>,
            SearchInventoryItemsQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<DeleteInventoryItemCommand, Result>,
            DeleteInventoryItemCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<AdjustInventoryStockCommand, Result<InventoryTransactionDto>>,
            AdjustInventoryStockCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetInventoryStockQuery, Result<int>>,
            GetInventoryStockQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateSupplierCommand, Result<SupplierDto>>,
            CreateSupplierCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetSupplierQuery, Result<SupplierDto>>,
            GetSupplierQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchSuppliersQuery, PagedResult<SupplierDto>>,
            SearchSuppliersQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<DeleteSupplierCommand, Result>,
            DeleteSupplierCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateInsurancePolicyCommand, Result<InsurancePolicyDto>>,
            CreateInsurancePolicyCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchInsurancePoliciesQuery, PagedResult<InsurancePolicyDto>>,
            SearchInsurancePoliciesQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateStaffCommand, Result<StaffDto>>,
            CreateStaffCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchStaffQuery, PagedResult<StaffDto>>,
            SearchStaffQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateSubscriptionCommand, Result<SubscriptionDto>>,
            CreateSubscriptionCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchSubscriptionsQuery, PagedResult<SubscriptionDto>>,
            SearchSubscriptionsQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateDocumentCommand, Result<DocumentDto>>,
            CreateDocumentCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchDocumentsQuery, PagedResult<DocumentDto>>,
            SearchDocumentsQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<DeleteDocumentCommand, Result>,
            DeleteDocumentCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateNotificationCommand, Result<NotificationDto>>,
            CreateNotificationCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchNotificationsQuery, PagedResult<NotificationDto>>,
            SearchNotificationsQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchAuditLogsQuery, PagedResult<AuditLogDto>>,
            SearchAuditLogsQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>,
            GetDashboardSummaryQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateTenantCommand, Result<TenantDto>>,
            CreateTenantCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateRoleCommand, Result<RoleDto>>,
            CreateRoleCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchRolesQuery, PagedResult<RoleDto>>,
            SearchRolesQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateUserCommand, Result<UserDto>>,
            CreateUserCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchUsersQuery, PagedResult<UserDto>>,
            SearchUsersQueryHandler
        >();

        services.AddScoped<
            IRequestHandler<LoginCommand, Result<AuthTokens>>,
            LoginCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<RefreshTokenCommand, Result<AuthTokens>>,
            RefreshTokenCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<RevokeRefreshTokenCommand, Result>,
            RevokeRefreshTokenCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<ChangePasswordCommand, Result>,
            ChangePasswordCommandHandler
        >();

        services.AddScoped<
            IRequestHandler<UpdateAppointmentCommand, Result<AppointmentDto>>,
            UpdateAppointmentCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetAdmissionHistoryQuery, IReadOnlyList<AdmissionHistoryDto>>,
            GetAdmissionHistoryQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<AddDiagnosisCommand, Result<MedicalRecordDto>>,
            AddDiagnosisCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateLabTestCommand, Result<LabTestDto>>,
            CreateLabTestCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetLabTestQuery, Result<LabTestDto>>,
            GetLabTestQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchLabTestsQuery, PagedResult<LabTestDto>>,
            SearchLabTestsQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<ChangeLabOrderStatusCommand, Result<LabOrderDto>>,
            ChangeLabOrderStatusCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetLabResultHistoryQuery, IReadOnlyList<LabResultDto>>,
            GetLabResultHistoryQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreateRadiologyTestCommand, Result<RadiologyTestDto>>,
            CreateRadiologyTestCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetRadiologyTestQuery, Result<RadiologyTestDto>>,
            GetRadiologyTestQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchRadiologyTestsQuery, PagedResult<RadiologyTestDto>>,
            SearchRadiologyTestsQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<GetRadiologyResultHistoryQuery, IReadOnlyList<RadiologyResultDto>>,
            GetRadiologyResultHistoryQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<CreatePurchaseOrderCommand, Result<PurchaseOrderDto>>,
            CreatePurchaseOrderCommandHandler
        >();
        services.AddScoped<
            IRequestHandler<GetPurchaseOrderQuery, Result<PurchaseOrderDto>>,
            GetPurchaseOrderQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<SearchPurchaseOrdersQuery, PagedResult<PurchaseOrderDto>>,
            SearchPurchaseOrdersQueryHandler
        >();
        services.AddScoped<
            IRequestHandler<ChangePurchaseOrderStatusCommand, Result<PurchaseOrderDto>>,
            ChangePurchaseOrderStatusCommandHandler
        >();

        return services;
    }
}
