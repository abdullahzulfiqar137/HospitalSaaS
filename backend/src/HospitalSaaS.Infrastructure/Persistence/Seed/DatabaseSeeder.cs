using HospitalSaaS.Application.Common.Security;
using HospitalSaaS.Domain.Admissions;
using HospitalSaaS.Domain.Appointments;
using HospitalSaaS.Domain.Billing;
using HospitalSaaS.Domain.Departments;
using HospitalSaaS.Domain.Doctors;
using HospitalSaaS.Domain.Documents;
using HospitalSaaS.Domain.Insurance;
using HospitalSaaS.Domain.Inventory;
using HospitalSaaS.Domain.Laboratory;
using HospitalSaaS.Domain.MedicalRecords;
using HospitalSaaS.Domain.Notifications;
using HospitalSaaS.Domain.Patients;
using HospitalSaaS.Domain.Payments;
using HospitalSaaS.Domain.Pharmacy;
using HospitalSaaS.Domain.Radiology;
using HospitalSaaS.Domain.Roles;
using HospitalSaaS.Domain.Staff;
using HospitalSaaS.Domain.Subscriptions;
using HospitalSaaS.Domain.Suppliers;
using HospitalSaaS.Domain.Tenants;
using HospitalSaaS.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HospitalSaaS.Infrastructure.Persistence.Seed;

public sealed class DatabaseSeeder(
    HospitalDbContext db,
    IPasswordHasher<User> hasher,
    IConfiguration configuration
)
{
    public async Task SeedAsync(CancellationToken ct = default)
    {
        // 1. Seed Permissions
        foreach (var name in Permissions.All)
            if (!await db.Permissions.IgnoreQueryFilters().AnyAsync(x => x.Name == name, ct))
                db.Permissions.Add(new Permission(name));
        await db.SaveChangesAsync(ct);

        var tenantName = configuration["InitialAdmin:TenantName"] ?? "St. Jude Hospital";
        var slug = configuration["InitialAdmin:TenantSlug"] ?? "initial-hospital";
        var defaultPassword = configuration["InitialAdmin:Password"] ?? "Password123!";

        // 2. Seed Tenant
        var hospital = await db.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(ct);
        if (hospital is null)
        {
            hospital = new Tenant(tenantName, slug);
            db.Tenants.Add(hospital);
            await db.SaveChangesAsync(ct);
        }

        var allPermissions = await db.Permissions.IgnoreQueryFilters().ToListAsync(ct);

        // 3. Seed Roles
        var roleNames = new[] { "Hospital Admin", "Doctor", "Supplier", "Buyer", "Lab Technician", "Pharmacist", "Patient" };
        var roleMap = new Dictionary<string, Role>();

        foreach (var rName in roleNames)
        {
            var role = await db.Roles.IgnoreQueryFilters()
                .Include(x => x.Permissions)
                .FirstOrDefaultAsync(x => x.TenantId == hospital.Id && x.Name == rName, ct);

            if (role is null)
            {
                role = new Role(hospital.Id, rName);
                if (rName == "Hospital Admin")
                {
                    foreach (var perm in allPermissions) role.Grant(perm);
                }
                else
                {
                    foreach (var perm in allPermissions)
                    {
                        if (rName == "Doctor" && (perm.Name.StartsWith("Patients") || perm.Name.StartsWith("Doctors") || perm.Name.StartsWith("Appointments") || perm.Name.StartsWith("Admissions") || perm.Name.StartsWith("MedicalRecords") || perm.Name.StartsWith("Prescriptions") || perm.Name.StartsWith("Laboratory") || perm.Name.StartsWith("Radiology")))
                            role.Grant(perm);
                        else if (rName == "Supplier" && (perm.Name.StartsWith("Inventory") || perm.Name.StartsWith("Suppliers") || perm.Name.StartsWith("PurchaseOrders")))
                            role.Grant(perm);
                        else if (rName == "Buyer" && (perm.Name.StartsWith("Inventory") || perm.Name.StartsWith("Suppliers") || perm.Name.StartsWith("PurchaseOrders") || perm.Name.StartsWith("Billing")))
                            role.Grant(perm);
                        else if (rName == "Lab Technician" && (perm.Name.StartsWith("Laboratory") || perm.Name.StartsWith("LabTests") || perm.Name.StartsWith("MedicalRecords")))
                            role.Grant(perm);
                        else if (rName == "Pharmacist" && (perm.Name.StartsWith("Pharmacy") || perm.Name.StartsWith("Inventory") || perm.Name.StartsWith("Prescriptions")))
                            role.Grant(perm);
                        else if (rName == "Patient" && (perm.Name.StartsWith("Appointments") || perm.Name.StartsWith("Prescriptions") || perm.Name.StartsWith("Laboratory") || perm.Name.StartsWith("Billing")))
                            role.Grant(perm);
                    }
                }
                db.Roles.Add(role);
            }
            roleMap[rName] = role;
        }
        await db.SaveChangesAsync(ct);

        // 4. Seed Users
        var usersToSeed = new (string Email, string RoleName)[]
        {
            ("admin@hospital.com", "Hospital Admin"),
            ("doctor@hospital.com", "Doctor"),
            ("supplier@hospital.com", "Supplier"),
            ("buyer@hospital.com", "Buyer"),
            ("labtech@hospital.com", "Lab Technician"),
            ("pharmacist@hospital.com", "Pharmacist"),
            ("patient@hospital.com", "Patient")
        };

        var userMap = new Dictionary<string, User>();
        foreach (var (email, roleName) in usersToSeed)
        {
            var normalized = email.ToLowerInvariant();
            var existingUser = await db.Users.IgnoreQueryFilters()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Email == normalized, ct);

            if (existingUser is null)
            {
                existingUser = new User(hospital.Id, email, string.Empty);
                var hashedPassword = hasher.HashPassword(existingUser, defaultPassword);
                existingUser.ChangePassword(hashedPassword);
                if (roleMap.TryGetValue(roleName, out var assignedRole))
                {
                    existingUser.AssignRole(assignedRole);
                }
                db.Users.Add(existingUser);
            }
            userMap[email] = existingUser;
        }
        await db.SaveChangesAsync(ct);

        // 5. Seed Departments
        var cardDept = await db.Departments.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.Code == "CARD", ct);
        if (cardDept is null)
        {
            cardDept = new Department(hospital.Id, "Cardiology", "CARD", "Cardiovascular health and surgery department");
            db.Departments.Add(cardDept);
        }
        var neurDept = await db.Departments.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.Code == "NEUR", ct);
        if (neurDept is null)
        {
            neurDept = new Department(hospital.Id, "Neurology", "NEUR", "Brain, spinal cord and nerves care");
            db.Departments.Add(neurDept);
        }
        var orthDept = await db.Departments.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.Code == "ORTH", ct);
        if (orthDept is null)
        {
            orthDept = new Department(hospital.Id, "Orthopedics", "ORTH", "Bones, joints, and muscular system");
            db.Departments.Add(orthDept);
        }
        var pedDept = await db.Departments.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.Code == "PED", ct);
        if (pedDept is null)
        {
            pedDept = new Department(hospital.Id, "Pediatrics", "PED", "Infants, children, and adolescent care");
            db.Departments.Add(pedDept);
        }
        var emrgDept = await db.Departments.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.Code == "EMRG", ct);
        if (emrgDept is null)
        {
            emrgDept = new Department(hospital.Id, "Emergency Care", "EMRG", "24/7 critical emergency and trauma unit");
            db.Departments.Add(emrgDept);
        }
        var genDept = await db.Departments.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.Code == "GEN", ct);
        if (genDept is null)
        {
            genDept = new Department(hospital.Id, "General Medicine", "GEN", "Primary healthcare and general practice");
            db.Departments.Add(genDept);
        }
        await db.SaveChangesAsync(ct);

        // 6. Seed Staff Members
        if (!await db.StaffMembers.IgnoreQueryFilters().AnyAsync(ct))
        {
            var s1 = new StaffMember(hospital.Id, "STF-501", "David", "Miller", "Head Nurse", cardDept.Id);
            var s2 = new StaffMember(hospital.Id, "STF-502", "Rachel", "Green", "Senior Lab Technician", emrgDept.Id);
            var s3 = new StaffMember(hospital.Id, "STF-503", "James", "Wilson", "Chief Pharmacist", cardDept.Id);
            var s4 = new StaffMember(hospital.Id, "STF-504", "Maria", "Garcia", "Front Desk Receptionist", emrgDept.Id);

            db.StaffMembers.AddRange(s1, s2, s3, s4);
            await db.SaveChangesAsync(ct);
        }

        // 7. Seed Doctors
        if (!await db.Doctors.IgnoreQueryFilters().AnyAsync(ct))
        {
            var doc1 = new Doctor(hospital.Id, "DOC-101", "Sarah", "Ahmed", "Cardiologist", cardDept.Id, "LIC-99881", "0300 1122334", "sarah.ahmed@hospital.com", 3500m);
            var doc2 = new Doctor(hospital.Id, "DOC-102", "Muhammad", "Ali", "Neurologist", neurDept.Id, "LIC-99882", "0321 2233445", "doctor@hospital.com", 4000m);
            var doc3 = new Doctor(hospital.Id, "DOC-103", "Tariq", "Mahmood", "Orthopedic Surgeon", orthDept.Id, "LIC-99883", "0333 3344556", "tariq.mahmood@hospital.com", 3000m);
            var doc4 = new Doctor(hospital.Id, "DOC-104", "Ayesha", "Khan", "Pediatrician", pedDept.Id, "LIC-99884", "0345 4455667", "ayesha.khan@hospital.com", 2500m);
            var doc5 = new Doctor(hospital.Id, "DOC-105", "Usman", "Ghani", "General Physician", genDept.Id, "LIC-99885", "0312 5566778", "usman.ghani@hospital.com", 2000m);

            db.Doctors.AddRange(doc1, doc2, doc3, doc4, doc5);
            await db.SaveChangesAsync(ct);
        }

        // 8. Seed Patients
        if (!await db.Patients.IgnoreQueryFilters().AnyAsync(ct))
        {
            var p1 = new Patient(hospital.Id, "PAT-2026-001001", "Tariq", "Mehmood", "Vance", new DateOnly(1988, 5, 14), "Male", "0300 1234567", "patient@hospital.com", "42101-1234567-1", "House 42, Block 6, PECHS, Karachi", "Farhan Vance", "0300 7654321", "Married", "Routine cardiac checkup");
            var p2 = new Patient(hospital.Id, "PAT-2026-001002", "Zainab", null, "Bibi", new DateOnly(1995, 11, 22), "Female", "0321 9876543", "zainab.bibi@example.com", "42201-9876543-2", "Flat 3B, Gulshan Executive Towers, Karachi", "Rashid Ahmed", "0321 1122334", "Married", "Migraine consultation");
            var p3 = new Patient(hospital.Id, "PAT-2026-001003", "Bilal", null, "Hassan", new DateOnly(1992, 3, 10), "Male", "0333 4567890", "bilal.hassan@example.com", "42301-4567890-3", "Street 12, F-8/3, Islamabad", "Asad Hassan", "0333 9988776", "Single", "Knee joint pain post injury");
            var p4 = new Patient(hospital.Id, "PAT-2026-001004", "Maria", null, "Ahmed", new DateOnly(2018, 8, 5), "Female", "0345 2345678", "maria.parent@example.com", "42401-2345678-4", "Sector 11-A, North Nazimabad, Karachi", "Hamid Ahmed", "0345 2345678", "Single", "Pediatric vaccination check");
            var p5 = new Patient(hospital.Id, "PAT-2026-001005", "Hamza", null, "Sheikh", new DateOnly(1980, 1, 30), "Male", "0312 8765432", "hamza.sheikh@example.com", "42501-8765432-5", "Phase 5, DHA, Lahore", "Kashif Sheikh", "0312 3344556", "Married", "Annual physical checkup");

            db.Patients.AddRange(p1, p2, p3, p4, p5);
            await db.SaveChangesAsync(ct);
        }

        // 9. Seed Wards, Rooms, and Beds
        if (!await db.Wards.IgnoreQueryFilters().AnyAsync(ct))
        {
            var w1 = new Ward(hospital.Id, "General Medical Ward A", "WARD-GEN-A");
            var w2 = new Ward(hospital.Id, "Intensive Care Unit (ICU)", "WARD-ICU-1");
            var w3 = new Ward(hospital.Id, "Cardiac Care Ward", "WARD-CCU-2");

            db.Wards.AddRange(w1, w2, w3);
            await db.SaveChangesAsync(ct);

            var room1 = new Room(hospital.Id, w1.Id, "Room 101");
            var room2 = new Room(hospital.Id, w2.Id, "Room 201 (ICU)");
            db.Rooms.AddRange(room1, room2);
            await db.SaveChangesAsync(ct);

            var bed1 = new Bed(hospital.Id, room1.Id, "Bed 101-A");
            var bed2 = new Bed(hospital.Id, room1.Id, "Bed 101-B");
            var bed3 = new Bed(hospital.Id, room2.Id, "ICU Bed 01");
            db.Beds.AddRange(bed1, bed2, bed3);
            await db.SaveChangesAsync(ct);

            var p1 = await db.Patients.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.PatientNumber == "PAT-2026-001001", ct);
            var doc1 = await db.Doctors.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.EmployeeNumber == "DOC-101", ct);

            if (p1 is not null && doc1 is not null)
            {
                var adm = new Admission(hospital.Id, "ADM-2026-001", p1.Id, doc1.Id, "Emergency Inpatient", "Acute Angina Observation", w3.Id, room1.Id, bed1.Id);
                bed1.Occupy();
                db.Admissions.Add(adm);
                await db.SaveChangesAsync(ct);
            }
        }

        // 10. Seed Appointments
        if (!await db.Appointments.IgnoreQueryFilters().AnyAsync(ct))
        {
            var p1 = await db.Patients.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.PatientNumber == "PAT-2026-001001", ct);
            var p2 = await db.Patients.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.PatientNumber == "PAT-2026-001002", ct);
            var p3 = await db.Patients.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.PatientNumber == "PAT-2026-001003", ct);

            var docCard = await db.Doctors.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.EmployeeNumber == "DOC-101", ct);
            var docNeur = await db.Doctors.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.EmployeeNumber == "DOC-102", ct);
            var docOrth = await db.Doctors.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.EmployeeNumber == "DOC-103", ct);

            if (p1 is not null && docCard is not null)
            {
                var app1 = new Appointment(hospital.Id, p1.Id, docCard.Id, docCard.DepartmentId, DateTime.UtcNow.AddDays(1).AddHours(10), 30, "Consultation", "Follow up cardiac checkup", "Bring recent ECG report");
                db.Appointments.Add(app1);
            }
            if (p2 is not null && docNeur is not null)
            {
                var app2 = new Appointment(hospital.Id, p2.Id, docNeur.Id, docNeur.DepartmentId, DateTime.UtcNow.AddDays(2).AddHours(14), 30, "Consultation", "Severe headaches", "Patient requested afternoon slot");
                db.Appointments.Add(app2);
            }
            if (p3 is not null && docOrth is not null)
            {
                var app3 = new Appointment(hospital.Id, p3.Id, docOrth.Id, docOrth.DepartmentId, DateTime.UtcNow.AddDays(3).AddHours(11), 30, "Follow-up", "Knee joint pain evaluation", "X-Ray done");
                db.Appointments.Add(app3);
            }
            await db.SaveChangesAsync(ct);
        }

        // 11. Seed Medical Records & Diagnoses
        if (!await db.MedicalRecords.IgnoreQueryFilters().AnyAsync(ct))
        {
            var p1 = await db.Patients.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.PatientNumber == "PAT-2026-001001", ct);
            var doc1 = await db.Doctors.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.EmployeeNumber == "DOC-101", ct);

            if (p1 is not null && doc1 is not null)
            {
                var mr = new MedicalRecord(hospital.Id, p1.Id, doc1.Id, null, "Chest discomfort during physical exertion", "Mild hypertension history", "BP 130/85, HR 72, Normal S1/S2 heart sounds", "Routine cardiac notes", "Echocardiogram and Lipid profile ordered", new DateOnly(2026, 8, 25));
                mr.AddDiagnosis("I20.9", "Angina Pectoris, Unspecified", true);
                db.MedicalRecords.Add(mr);
                await db.SaveChangesAsync(ct);
            }
        }

        // 12. Seed Pharmacy Medicines
        if (!await db.Medicines.IgnoreQueryFilters().AnyAsync(ct))
        {
            var m1 = new Medicine(hospital.Id, "Amoxicillin 500mg", "Amoxicillin", "Amoxil", "500mg", "Capsule", "GSK Pharma", "8901234567891", 15.00m, 10.00m, 50);
            var m2 = new Medicine(hospital.Id, "Lipitor 20mg", "Atorvastatin", "Lipitor", "20mg", "Tablet", "Pfizer", "8901234567892", 30.00m, 20.00m, 30);
            var m3 = new Medicine(hospital.Id, "Glucophage 850mg", "Metformin", "Glucophage", "850mg", "Tablet", "Merck", "8901234567893", 20.00m, 12.00m, 40);
            var m4 = new Medicine(hospital.Id, "Panadol Extra", "Paracetamol", "Panadol", "500mg/65mg", "Tablet", "Haleon", "8901234567894", 5.00m, 3.00m, 100);

            db.Medicines.AddRange(m1, m2, m3, m4);
            await db.SaveChangesAsync(ct);
        }

        // 13. Seed Laboratory & Radiology Tests
        if (!await db.LabTests.IgnoreQueryFilters().AnyAsync(ct))
        {
            var lt1 = new LabTest(hospital.Id, "Complete Blood Count", "CBC-01", "Hematology", 500m, "4.5-11.0 k/uL", "k/uL");
            var lt2 = new LabTest(hospital.Id, "Lipid Profile Panel", "LIP-02", "Biochemistry", 1200m, "<200 mg/dL", "mg/dL");
            var lt3 = new LabTest(hospital.Id, "Blood Glucose Fasting", "BSF-03", "Endocrinology", 300m, "70-99 mg/dL", "mg/dL");

            db.LabTests.AddRange(lt1, lt2, lt3);
            await db.SaveChangesAsync(ct);
        }

        if (!await db.RadiologyTests.IgnoreQueryFilters().AnyAsync(ct))
        {
            var rt1 = new RadiologyTest(hospital.Id, "CXR-PA", "Chest X-Ray PA View", "X-Ray", 1500m);
            var rt2 = new RadiologyTest(hospital.Id, "MRI-BRAIN", "Brain MRI with Contrast", "MRI", 12000m);
            var rt3 = new RadiologyTest(hospital.Id, "CT-ABD", "Abdomen & Pelvis CT Scan", "CT Scan", 8000m);

            db.RadiologyTests.AddRange(rt1, rt2, rt3);
            await db.SaveChangesAsync(ct);
        }

        // 14. Seed Lab & Radiology Orders
        if (!await db.LabOrders.IgnoreQueryFilters().AnyAsync(ct))
        {
            var p1 = await db.Patients.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.PatientNumber == "PAT-2026-001001", ct);
            var doc1 = await db.Doctors.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.EmployeeNumber == "DOC-101", ct);
            var lt2 = await db.LabTests.IgnoreQueryFilters().FirstOrDefaultAsync(l => l.Code == "LIP-02", ct);

            if (p1 is not null && doc1 is not null && lt2 is not null)
            {
                var lo = new LabOrder(hospital.Id, p1.Id, doc1.Id);
                lo.AddTest(lt2.Code, lt2.Name, lt2.Id);
                db.LabOrders.Add(lo);
                await db.SaveChangesAsync(ct);
            }
        }

        if (!await db.RadiologyOrders.IgnoreQueryFilters().AnyAsync(ct))
        {
            var p1 = await db.Patients.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.PatientNumber == "PAT-2026-001001", ct);
            var doc1 = await db.Doctors.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.EmployeeNumber == "DOC-101", ct);
            var rt1 = await db.RadiologyTests.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Code == "CXR-PA", ct);

            if (p1 is not null && doc1 is not null && rt1 is not null)
            {
                var ro = new RadiologyOrder(hospital.Id, p1.Id, doc1.Id, "Evaluate lung fields and cardiac size", rt1.Id);
                db.RadiologyOrders.Add(ro);
                await db.SaveChangesAsync(ct);
            }
        }

        // 15. Seed Suppliers & Inventory Items
        if (!await db.Suppliers.IgnoreQueryFilters().AnyAsync(ct))
        {
            var sup1 = new Supplier(hospital.Id, "MedPharma Supplies Ltd", "Mark Stevens", "0300 9988771", "orders@medpharma.com", "Plot 14, Industrial Zone, Karachi");
            var sup2 = new Supplier(hospital.Id, "Apex Surgical Supplies", "Linda Vance", "0321 8877662", "sales@apexsurgical.com", "Main Boulevard, Gulberg, Lahore");

            db.Suppliers.AddRange(sup1, sup2);
            await db.SaveChangesAsync(ct);
        }

        if (!await db.InventoryItems.IgnoreQueryFilters().AnyAsync(ct))
        {
            var item1 = new InventoryItem(hospital.Id, "SKU-GLV-M", "Surgical Gloves (Medium)", "Box (100)", 50);
            var item2 = new InventoryItem(hospital.Id, "SKU-SYR-5ML", "Sterile Syringes 5ml", "Pack (50)", 100);
            var item3 = new InventoryItem(hospital.Id, "SKU-SAL-500ML", "Normal Saline 0.9% 500ml", "Bag", 80);

            db.InventoryItems.AddRange(item1, item2, item3);
            await db.SaveChangesAsync(ct);
        }

        // 16. Seed Invoices & Payments
        if (!await db.Invoices.IgnoreQueryFilters().AnyAsync(ct))
        {
            var p1 = await db.Patients.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.PatientNumber == "PAT-2026-001001", ct);

            if (p1 is not null)
            {
                var inv = new Invoice(hospital.Id, "INV-2026-00101", p1.Id);
                inv.AddItem("Cardiology Consultation Fee", 1, 3500m);
                inv.AddItem("Lipid Profile Lab Test", 1, 1200m);
                inv.Issue();
                inv.MarkPaid();

                db.Invoices.Add(inv);
                await db.SaveChangesAsync(ct);

                var pay = new Payment(hospital.Id, inv.Id, "PAY-2026-00101", inv.Total, "Credit Card");
                db.Payments.Add(pay);
                await db.SaveChangesAsync(ct);
            }
        }

        // 17. Seed Insurance Policies
        if (!await db.InsurancePolicies.IgnoreQueryFilters().AnyAsync(ct))
        {
            var p1 = await db.Patients.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.PatientNumber == "PAT-2026-001001", ct);
            if (p1 is not null)
            {
                var ins = new InsurancePolicy(hospital.Id, p1.Id, "Jubilee Life Insurance", "JUB-POL-99210", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
                db.InsurancePolicies.Add(ins);
                await db.SaveChangesAsync(ct);
            }
        }

        // 18. Seed Subscriptions & Notifications & Documents
        if (!await db.TenantSubscriptions.IgnoreQueryFilters().AnyAsync(ct))
        {
            var sub = new TenantSubscription(hospital.Id, "Enterprise Hospital Tier", DateTime.UtcNow.AddMonths(-6), DateTime.UtcNow.AddMonths(6));
            db.TenantSubscriptions.Add(sub);
            await db.SaveChangesAsync(ct);
        }

        if (!await db.Notifications.IgnoreQueryFilters().AnyAsync(ct))
        {
            if (userMap.TryGetValue("admin@hospital.com", out var adminUser))
            {
                var notif1 = new Notification(hospital.Id, adminUser.Id, "System Backup Complete", "Daily database backup was completed successfully.", "In-App");
                notif1.MarkSent();
                var notif2 = new Notification(hospital.Id, adminUser.Id, "New Patient Admission", "Patient Tariq Vance admitted to Ward-CCU-2 Bed 101-A.", "In-App");
                notif2.MarkSent();

                db.Notifications.AddRange(notif1, notif2);
                await db.SaveChangesAsync(ct);
            }
        }

        if (!await db.Documents.IgnoreQueryFilters().AnyAsync(ct))
        {
            var p1 = await db.Patients.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.PatientNumber == "PAT-2026-001001", ct);
            if (p1 is not null)
            {
                var doc = new Document(hospital.Id, p1.Id, "Cardiac_Consent_Form.pdf", "application/pdf", 245000, "docs/patient-001/consent.pdf");
                db.Documents.Add(doc);
                await db.SaveChangesAsync(ct);
            }
        }
    }
}
