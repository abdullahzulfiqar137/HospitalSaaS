using HospitalSaaS.Domain.Appointments;
using HospitalSaaS.Domain.Exceptions;
using HospitalSaaS.Domain.Inventory;
using HospitalSaaS.Domain.Laboratory;
using HospitalSaaS.Domain.MedicalRecords;
using HospitalSaaS.Domain.Pharmacy;
using HospitalSaaS.Domain.Radiology;

namespace HospitalSaaS.Domain.Tests;

public sealed class CompletedModuleTests
{
    [Fact]
    public void Appointment_update_changes_editable_details()
    {
        var appointment = CreateAppointment();
        var doctorId = Guid.NewGuid();

        appointment.Update(
            doctorId,
            Guid.NewGuid(),
            DateTime.SpecifyKind(DateTime.UtcNow.AddDays(2), DateTimeKind.Utc),
            45,
            "Follow-up",
            "Review",
            "Bring reports"
        );

        Assert.Equal(doctorId, appointment.DoctorId);
        Assert.Equal(45, appointment.DurationMinutes);
        Assert.Equal("Follow-up", appointment.AppointmentType);
    }

    [Fact]
    public void Completed_appointment_cannot_be_updated()
    {
        var appointment = CreateAppointment();
        appointment.CheckIn();
        appointment.Start();
        appointment.Complete();

        Assert.Throws<DomainException>(() =>
            appointment.Update(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.SpecifyKind(DateTime.UtcNow.AddDays(2), DateTimeKind.Utc),
                30,
                "Review",
                "Reason",
                null
            )
        );
    }

    [Fact]
    public void Medical_record_allows_only_one_primary_diagnosis()
    {
        var record = new MedicalRecord(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            "Complaint",
            null,
            null,
            null,
            null,
            null
        );
        record.AddDiagnosis("A01", "Primary", true);

        Assert.Throws<DomainException>(() => record.AddDiagnosis("B02", "Another", true));
    }

    [Fact]
    public void Lab_order_enforces_sample_workflow()
    {
        var order = new LabOrder(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        order.AddTest("CBC", "Complete blood count");

        Assert.Throws<DomainException>(order.StartProcessing);
        order.MarkSampleCollected();
        order.StartProcessing();

        Assert.Equal(LabOrderStatus.Processing, order.Status);
    }

    [Fact]
    public void Purchase_order_calculates_total_and_receives_once()
    {
        var order = new PurchaseOrder(Guid.NewGuid(), "PO-1", Guid.NewGuid(), null, null);
        order.AddItem(Guid.NewGuid(), 3, 12.50m);
        order.Submit();
        order.Receive();

        Assert.Equal(37.50m, order.Total);
        Assert.Equal(PurchaseOrderStatus.Received, order.Status);
        Assert.Throws<DomainException>(order.Receive);
    }

    [Fact]
    public void Catalog_entities_preserve_financial_precision_and_codes()
    {
        var lab = new LabTest(
            Guid.NewGuid(),
            "Blood test",
            " cbc ",
            "Hematology",
            19.95m,
            null,
            null
        );
        var radiology = new RadiologyTest(Guid.NewGuid(), " xr-1 ", "X-Ray", "XR", 50.25m);

        Assert.Equal("CBC", lab.Code);
        Assert.Equal(19.95m, lab.Price);
        Assert.Equal("XR-1", radiology.Code);
        Assert.Equal(50.25m, radiology.Price);
    }

    [Fact]
    public void Pharmacy_supports_damage_and_expired_transactions()
    {
        var tenantId = Guid.NewGuid();
        var medicineId = Guid.NewGuid();

        var damage = new StockTransaction(
            tenantId,
            medicineId,
            StockTransactionType.Damage,
            -1,
            "Broken"
        );
        var expired = new StockTransaction(
            tenantId,
            medicineId,
            StockTransactionType.Expired,
            -2,
            "Expired lot"
        );

        Assert.Equal(StockTransactionType.Damage, damage.Type);
        Assert.Equal(StockTransactionType.Expired, expired.Type);
    }

    private static Appointment CreateAppointment() =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.SpecifyKind(DateTime.UtcNow.AddDays(1), DateTimeKind.Utc),
            30,
            "Consultation",
            "Checkup",
            null
        );
}
