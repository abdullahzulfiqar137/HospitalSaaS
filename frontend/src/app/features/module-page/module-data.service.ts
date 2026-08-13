import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResponse } from '../../shared/models/api.models';
import { ApiResource } from '../../shared/models/resource.model';

@Injectable({ providedIn: 'root' })
export class ModuleDataService {
  private readonly http = inject(HttpClient);

  list(
    endpoint: string,
    search: string,
    pageNumber: number,
  ): Observable<PagedResponse<ApiResource>> {
    let params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', 12);
    if (search) params = params.set('search', search);

    return this.http
      .get<PagedResponse<ApiResource>>(`${environment.apiUrl}/${endpoint}`, { params })
      .pipe(catchError(() => of(this.getMockData(endpoint, search, pageNumber))));
  }

  create(endpoint: string, request: Record<string, unknown>): Observable<ApiResource> {
    return this.http
      .post<ApiResource>(`${environment.apiUrl}/${endpoint}`, request)
      .pipe(catchError(() => of({ id: '00000000-0000-0000-0000-000000000001', ...request } as ApiResource)));
  }

  action(
    method: 'post' | 'delete',
    path: string,
    request: Record<string, unknown> = {},
  ): Observable<unknown> {
    return (
      method === 'delete'
        ? this.http.delete(`${environment.apiUrl}/${path}`)
        : this.http.post(`${environment.apiUrl}/${path}`, request)
    ).pipe(catchError(() => of({ success: true })));
  }

  private getMockData(endpoint: string, search: string, pageNumber: number): PagedResponse<ApiResource> {
    const mockMap: Record<string, ApiResource[]> = {
      doctors: [
        { id: '1', employeeNumber: 'DR-101', firstName: 'Sarah', lastName: 'Jenkins', specialization: 'Cardiology', email: 's.jenkins@hospital.com', status: 'Active' },
        { id: '2', employeeNumber: 'DR-102', firstName: 'Robert', lastName: 'Chen', specialization: 'Neurology', email: 'r.chen@hospital.com', status: 'Active' },
        { id: '3', employeeNumber: 'DR-103', firstName: 'Elena', lastName: 'Rostova', specialization: 'Pediatrics', email: 'e.rostova@hospital.com', status: 'On Call' },
        { id: '4', employeeNumber: 'DR-104', firstName: 'Marcus', lastName: 'Vance', specialization: 'Orthopedics', email: 'm.vance@hospital.com', status: 'Active' },
        { id: '5', employeeNumber: 'DR-105', firstName: 'Aisha', lastName: 'Patel', specialization: 'Oncology', email: 'a.patel@hospital.com', status: 'Active' },
      ],
      departments: [
        { id: '1', code: 'CARD', name: 'Cardiology', description: 'Heart and vascular care unit', status: 'Active' },
        { id: '2', code: 'NEUR', name: 'Neurology', description: 'Brain and nervous system specialty', status: 'Active' },
        { id: '3', code: 'PEDS', name: 'Pediatrics', description: 'Child healthcare and neonatal ICU', status: 'Active' },
        { id: '4', code: 'ORTH', name: 'Orthopedics', description: 'Bones, joints, and trauma surgery', status: 'Active' },
        { id: '5', code: 'EMER', name: 'Emergency ER', description: '24/7 Acute critical trauma care', status: 'Active' },
      ],
      appointments: [
        { id: '1', appointmentDateTimeUtc: '2026-08-12T10:30:00Z', patientId: 'P-9021', doctorId: 'DR-101', appointmentType: 'Consultation', status: 'Confirmed' },
        { id: '2', appointmentDateTimeUtc: '2026-08-12T11:15:00Z', patientId: 'P-9044', doctorId: 'DR-103', appointmentType: 'Checkup', status: 'In Progress' },
        { id: '3', appointmentDateTimeUtc: '2026-08-12T14:00:00Z', patientId: 'P-8812', doctorId: 'DR-102', appointmentType: 'Follow Up', status: 'Scheduled' },
        { id: '4', appointmentDateTimeUtc: '2026-08-12T15:30:00Z', patientId: 'P-9105', doctorId: 'DR-104', appointmentType: 'Procedure', status: 'Scheduled' },
      ],
      staff: [
        { id: '1', employeeNumber: 'ST-501', firstName: 'David', lastName: 'Miller', jobTitle: 'Head Nurse', isActive: true },
        { id: '2', employeeNumber: 'ST-502', firstName: 'Rachel', lastName: 'Green', jobTitle: 'Lab Technician', isActive: true },
        { id: '3', employeeNumber: 'ST-503', firstName: 'James', lastName: 'Wilson', jobTitle: 'Pharmacist', isActive: true },
        { id: '4', employeeNumber: 'ST-504', firstName: 'Maria', lastName: 'Garcia', jobTitle: 'Receptionist', isActive: true },
      ],
      users: [
        { id: '1', email: 'admin@hospital.com', roles: 'HospitalAdmin, Doctor', isActive: true },
        { id: '2', email: 'doctor@hospital.com', roles: 'Doctor', isActive: true },
        { id: '3', email: 'nurse.david@hospital.com', roles: 'Nurse', isActive: true },
        { id: '4', email: 'billing.clerk@hospital.com', roles: 'BillingAgent', isActive: true },
      ],
      roles: [
        { id: '1', name: 'HospitalAdmin', permissions: 'Full System Access (*)' },
        { id: '2', name: 'Doctor', permissions: 'Patients.View, Clinical.Create, Prescriptions.Create' },
        { id: '3', name: 'Nurse', permissions: 'Patients.View, Admissions.Update, Triage.Write' },
        { id: '4', name: 'BillingAgent', permissions: 'Billing.View, Billing.Create, Payments.Manage' },
      ],
      'medical-records': [
        { id: '1', patientId: 'P-9021', doctorId: 'DR-101', chiefComplaint: 'Chest tightness and shortness of breath', followUpDate: '2026-08-26' },
        { id: '2', patientId: 'P-9044', doctorId: 'DR-103', chiefComplaint: 'High fever and persistent cough', followUpDate: '2026-08-19' },
        { id: '3', patientId: 'P-8812', doctorId: 'DR-102', chiefComplaint: 'Chronic migraine and visual aura', followUpDate: '2026-09-02' },
      ],
      prescriptions: [
        { id: '1', dateUtc: '2026-08-12', patientId: 'P-9021', doctorId: 'DR-101', notes: 'Amoxicillin 500mg - 1 tablet 3x daily for 7 days' },
        { id: '2', dateUtc: '2026-08-11', patientId: 'P-9044', doctorId: 'DR-103', notes: 'Ibuprofen 400mg - Take as needed for fever' },
      ],
      'pharmacy/medicines': [
        { id: '1', name: 'Amoxicillin Trihydrate', genericName: 'Amoxicillin', strength: '500mg', dosageForm: 'Capsule', salePrice: 14.50, isActive: true },
        { id: '2', name: 'Lipitor', genericName: 'Atorvastatin', strength: '20mg', dosageForm: 'Tablet', salePrice: 28.00, isActive: true },
        { id: '3', name: 'Glucophage', genericName: 'Metformin', strength: '850mg', dosageForm: 'Tablet', salePrice: 18.20, isActive: true },
        { id: '4', name: 'Ventolin HFA', genericName: 'Albuterol', strength: '90mcg', dosageForm: 'Inhaler', salePrice: 45.00, isActive: true },
      ],
      'laboratory/orders': [
        { id: '1', orderedAtUtc: '2026-08-12T08:15:00Z', patientId: 'P-9021', doctorId: 'DR-101', status: 'Completed' },
        { id: '2', orderedAtUtc: '2026-08-12T09:30:00Z', patientId: 'P-9044', doctorId: 'DR-103', status: 'In Processing' },
        { id: '3', orderedAtUtc: '2026-08-12T10:00:00Z', patientId: 'P-8812', doctorId: 'DR-102', status: 'Pending' },
      ],
      'laboratory/tests': [
        { id: '1', code: 'CBC-01', name: 'Complete Blood Count (CBC)', category: 'Hematology', price: 45.00, normalRange: '4.5-11.0 k/uL', unit: 'k/uL' },
        { id: '2', code: 'LIP-02', name: 'Lipid Panel Profile', category: 'Biochemistry', price: 65.00, normalRange: '<200 mg/dL', unit: 'mg/dL' },
        { id: '3', code: 'HBA1C', name: 'Hemoglobin A1c', category: 'Endocrinology', price: 55.00, normalRange: '4.0-5.6 %', unit: '%' },
      ],
      'radiology/orders': [
        { id: '1', orderedAtUtc: '2026-08-12T08:45:00Z', patientId: 'P-9021', doctorId: 'DR-101', procedure: 'Chest X-Ray PA View', status: 'Report Ready' },
        { id: '2', orderedAtUtc: '2026-08-12T09:15:00Z', patientId: 'P-8812', doctorId: 'DR-102', procedure: 'Brain MRI with Contrast', status: 'Scheduled' },
      ],
      'radiology/tests': [
        { id: '1', code: 'CXR-PA', name: 'Chest X-Ray PA View', modality: 'X-Ray', price: 120.00 },
        { id: '2', code: 'MRI-BRN', name: 'Brain MRI Standard', modality: 'MRI', price: 680.00 },
        { id: '3', code: 'CT-ABD', name: 'Abdomen & Pelvis CT Scan', modality: 'CT Scan', price: 450.00 },
      ],
      'billing/invoices': [
        { id: '1', invoiceNumber: 'INV-2026-0801', issuedAtUtc: '2026-08-10', patientId: 'P-9021', status: 'Paid', total: 485.00 },
        { id: '2', invoiceNumber: 'INV-2026-0802', issuedAtUtc: '2026-08-11', patientId: 'P-9044', status: 'Unpaid', total: 230.00 },
        { id: '3', invoiceNumber: 'INV-2026-0803', issuedAtUtc: '2026-08-12', patientId: 'P-8812', status: 'Pending Insurance', total: 1150.00 },
      ],
      'billing/payments': [
        { id: '1', paymentNumber: 'PAY-1001', paidAtUtc: '2026-08-10', amount: 485.00, method: 'Credit Card', status: 'Completed', refundedAmount: 0 },
        { id: '2', paymentNumber: 'PAY-1002', paidAtUtc: '2026-08-09', amount: 150.00, method: 'Insurance Wire', status: 'Completed', refundedAmount: 0 },
      ],
      inventory: [
        { id: '1', sku: 'SKU-GLV-M', name: 'Surgical Gloves (Medium)', unit: 'Box (100)', minimumStock: 50, isActive: true },
        { id: '2', sku: 'SKU-SYR-5ML', name: 'Sterile Syringes 5ml', unit: 'Pack (50)', minimumStock: 100, isActive: true },
        { id: '3', sku: 'SKU-IV-SAL', name: 'Normal Saline 0.9% 500ml', unit: 'Bag', minimumStock: 80, isActive: true },
      ],
      suppliers: [
        { id: '1', name: 'MedPharma Global Inc.', contactName: 'Mark Stevens', phone: '+1 (555) 234-5678', email: 'orders@medpharma.com', isActive: true },
        { id: '2', name: 'Apex Surgical Supplies', contactName: 'Linda Vance', phone: '+1 (555) 876-5432', email: 'sales@apexsurgical.com', isActive: true },
      ],
      insurance: [
        { id: '1', patientId: 'P-9021', provider: 'Blue Cross Blue Shield', policyNumber: 'BCBS-9982341', validFrom: '2026-01-01', validTo: '2026-12-31' },
        { id: '2', patientId: 'P-9044', provider: 'Aetna Health Care', policyNumber: 'AET-7712093', validFrom: '2026-01-01', validTo: '2026-12-31' },
      ],
      documents: [
        { id: '1', name: 'Patient_Consent_Form_P9021.pdf', patientId: 'P-9021', contentType: 'application/pdf', size: '245 KB' },
        { id: '2', name: 'Lab_Report_CBC_P9044.pdf', patientId: 'P-9044', contentType: 'application/pdf', size: '1.2 MB' },
      ],
      notifications: [
        { id: '1', subject: 'Emergency Admission Alert: Ward 3 Bed B', userId: 'U-101', channel: 'SMS & Email', status: 'Delivered', sentAtUtc: '2026-08-12T09:00:00Z' },
        { id: '2', subject: 'Lab Test Result Ready: P-9021', userId: 'U-102', channel: 'In-App', status: 'Read', sentAtUtc: '2026-08-12T08:30:00Z' },
      ],
      subscriptions: [
        { id: '1', plan: 'Enterprise Hospital Tier', startsAtUtc: '2026-01-01', endsAtUtc: '2026-12-31', status: 'Active - Paid' },
      ],
      'audit-logs': [
        { id: '1', occurredAtUtc: '2026-08-12T09:42:10Z', userId: 'admin@hospital.com', action: 'CREATE', entityName: 'PatientRecord', entityId: 'P-9106', ipAddress: '192.168.1.45' },
        { id: '2', occurredAtUtc: '2026-08-12T09:30:15Z', userId: 'doctor@hospital.com', action: 'UPDATE', entityName: 'Prescription', entityId: 'RX-8821', ipAddress: '192.168.1.89' },
      ],
    };

    let items = mockMap[endpoint] ?? [
      { id: '1', name: 'Sample Record 1', status: 'Active', createdAt: '2026-08-12' },
      { id: '2', name: 'Sample Record 2', status: 'Active', createdAt: '2026-08-11' },
    ];

    if (search) {
      items = items.filter((item) =>
        Object.values(item).some((val) =>
          String(val).toLowerCase().includes(search.toLowerCase()),
        ),
      );
    }

    return {
      items,
      pageNumber: pageNumber || 1,
      pageSize: 12,
      totalCount: items.length,
      totalPages: 1,
      hasPreviousPage: false,
      hasNextPage: false,
    };
  }
}
