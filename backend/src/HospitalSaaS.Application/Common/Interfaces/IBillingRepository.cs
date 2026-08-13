using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Billing;
using HospitalSaaS.Domain.Payments;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IBillingRepository
{
    Task AddInvoiceAsync(Invoice x, CancellationToken ct);
    Task<Invoice?> GetInvoiceAsync(Guid id, bool tracking, CancellationToken ct);
    Task<PagedResult<Invoice>> SearchInvoicesAsync(
        Guid? patientId,
        InvoiceStatus? status,
        int page,
        int size,
        CancellationToken ct
    );
    Task AddPaymentAsync(Payment x, CancellationToken ct);
    Task<Payment?> GetPaymentAsync(Guid id, bool tracking, CancellationToken ct);
    Task<PagedResult<Payment>> SearchPaymentsAsync(
        Guid? invoiceId,
        int page,
        int size,
        CancellationToken ct
    );
    Task<decimal> GetNetPaidAsync(Guid invoiceId, CancellationToken ct);
    Task<int> CountInvoicesAsync(int year, CancellationToken ct);
    Task<int> CountPaymentsAsync(int year, CancellationToken ct);
    Task<bool> InvoiceNumberExistsAsync(string number, CancellationToken ct);
    Task<bool> PaymentNumberExistsAsync(string number, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
