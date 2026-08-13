export type FieldType =
  | 'text'
  | 'email'
  | 'password'
  | 'number'
  | 'date'
  | 'datetime-local'
  | 'textarea'
  | 'json'
  | 'boolean'
  | 'select'
  | 'csv'
  | 'guid-list';
export interface ModuleField {
  key: string;
  label: string;
  type: FieldType;
  required?: boolean;
  placeholder?: string;
  options?: string[];
}
export interface ModuleForm {
  endpoint: string;
  fields: ModuleField[];
}
const f = (
  key: string,
  label: string,
  type: FieldType = 'text',
  required = true,
  placeholder?: string,
): ModuleField => ({ key, label, type, required, placeholder });
export const MODULE_FORMS: Record<string, ModuleForm> = {
  doctors: {
    endpoint: 'doctors',
    fields: [
      f('employeeNumber', 'Employee number'),
      f('firstName', 'First name'),
      f('lastName', 'Last name'),
      {
        ...f('specialization', 'Specialization', 'select'),
        options: [
          'Cardiology & Cardiovascular Medicine',
          'Neurology & Neurosurgery',
          'Orthopedic Surgery & Traumatology',
          'Pediatrics & Neonatology',
          'Ophthalmology & Vision Care',
          'Pulmonology & Respiratory Care',
          'Hematology & Oncology',
          'General Internal Medicine',
          'General & Vascular Surgery',
          'Clinical Pathology & Diagnostics',
          'Diagnostic Radiology & Imaging',
          'Dermatology & Cosmetology',
          'Psychiatry & Behavioral Health',
          'Otolaryngology (ENT)',
          'Obstetrics & Gynecology',
        ],
      },
      f('departmentId', 'Department ID'),
      f('licenseNumber', 'License number'),
      f('phone', 'Phone', 'text', false),
      f('email', 'Email', 'email', false),
      f('consultationFee', 'Consultation fee', 'number'),
    ],
  },
  departments: {
    endpoint: 'departments',
    fields: [
      f('name', 'Name'),
      f('code', 'Code'),
      f('description', 'Description', 'textarea', false),
    ],
  },
  appointments: {
    endpoint: 'appointments',
    fields: [
      f('patientId', 'Patient ID'),
      f('doctorId', 'Doctor ID'),
      f('departmentId', 'Department ID'),
      f('appointmentDateTimeUtc', 'Appointment time', 'datetime-local'),
      f('durationMinutes', 'Duration in minutes', 'number'),
      {
        ...f('appointmentType', 'Appointment type', 'select'),
        options: [
          'Consultation',
          'FollowUp',
          'Emergency',
          'RoutineCheckup',
          'Diagnostic',
          'Telehealth',
        ],
      },
      f('reason', 'Reason'),
      f('notes', 'Notes', 'textarea', false),
    ],
  },
  staff: {
    endpoint: 'staff',
    fields: [
      f('employeeNumber', 'Employee number'),
      f('firstName', 'First name'),
      f('lastName', 'Last name'),
      {
        ...f('jobTitle', 'Job title', 'select'),
        options: [
          'Registered Nurse (RN)',
          'Senior Staff Nurse',
          'Clinical Nurse Specialist',
          'Medical Assistant',
          'Head Nurse / Supervisor',
          'Lab Technician',
          'Radiologic Technologist',
          'Pharmacy Specialist',
          'Medical Receptionist',
          'Billing & Claims Specialist',
          'Hospital Administrator',
          'Facility Manager',
        ],
      },
      f('departmentId', 'Department ID', 'text', false),
    ],
  },
  users: {
    endpoint: 'users',
    fields: [
      f('email', 'Email', 'email'),
      f('password', 'Password', 'password'),
      f('roleIds', 'Role IDs', 'guid-list', true, 'guid-1, guid-2'),
    ],
  },
  roles: {
    endpoint: 'roles',
    fields: [
      f('name', 'Role name'),
      f('permissions', 'Permissions', 'csv', true, 'Patients.View, Patients.Create'),
    ],
  },
  pharmacy: {
    endpoint: 'pharmacy/medicines',
    fields: [
      f('name', 'Name'),
      f('genericName', 'Generic name'),
      f('brandName', 'Brand name', 'text', false),
      f('strength', 'Strength'),
      {
        ...f('dosageForm', 'Dosage form', 'select'),
        options: [
          'Tablet',
          'Capsule',
          'Syrup',
          'Injection',
          'Cream',
          'Inhaler',
          'Drops',
          'Ointment',
          'IV Fluid',
        ],
      },
      f('manufacturer', 'Manufacturer', 'text', false),
      f('barcode', 'Barcode', 'text', false),
      f('salePrice', 'Sale price', 'number'),
      f('purchasePrice', 'Purchase price', 'number'),
      f('minimumStockLevel', 'Minimum stock', 'number'),
    ],
  },
  inventory: {
    endpoint: 'inventory',
    fields: [
      f('sku', 'SKU'),
      f('name', 'Name'),
      {
        ...f('unit', 'Unit', 'select'),
        options: ['Box', 'Pack', 'Bottle', 'Piece', 'Vial', 'Ampoule', 'Strip', 'Kg', 'Litre'],
      },
      f('minimumStock', 'Minimum stock', 'number'),
    ],
  },
  suppliers: {
    endpoint: 'suppliers',
    fields: [
      f('name', 'Name'),
      f('contactName', 'Contact name', 'text', false),
      f('phone', 'Phone', 'text', false),
      f('email', 'Email', 'email', false),
      f('address', 'Address', 'textarea', false),
    ],
  },
  insurance: {
    endpoint: 'insurance',
    fields: [
      f('patientId', 'Patient ID'),
      f('provider', 'Provider'),
      f('policyNumber', 'Policy number'),
      f('validFrom', 'Valid from', 'date'),
      f('validTo', 'Valid to', 'date'),
    ],
  },
  notifications: {
    endpoint: 'notifications',
    fields: [
      f('userId', 'User ID'),
      f('subject', 'Subject'),
      f('body', 'Message', 'textarea'),
      { ...f('channel', 'Channel', 'select'), options: ['Email', 'Sms', 'InApp'] },
    ],
  },
  subscriptions: {
    endpoint: 'subscriptions',
    fields: [
      {
        ...f('plan', 'Plan', 'select'),
        options: ['Community', 'Professional', 'Enterprise'],
      },
      f('startsAtUtc', 'Starts at', 'datetime-local'),
      f('endsAtUtc', 'Ends at', 'datetime-local'),
    ],
  },
  'medical-records': {
    endpoint: 'medical-records',
    fields: [
      f('patientId', 'Patient ID'),
      f('doctorId', 'Doctor ID'),
      f('appointmentId', 'Appointment ID', 'text', false),
      f('chiefComplaint', 'Chief complaint'),
      f('history', 'History', 'textarea', false),
      f('examination', 'Examination', 'textarea', false),
      f('clinicalNotes', 'Clinical notes', 'textarea', false),
      f('treatmentPlan', 'Treatment plan', 'textarea', false),
      f('followUpDate', 'Follow-up date', 'date', false),
      f('diagnoses', 'Diagnoses', 'json', false, '[{"code":"","description":"","isPrimary":true}]'),
    ],
  },
  prescriptions: {
    endpoint: 'prescriptions',
    fields: [
      f('patientId', 'Patient ID'),
      f('doctorId', 'Doctor ID'),
      f('appointmentId', 'Appointment ID', 'text', false),
      f('notes', 'Notes', 'textarea', false),
      f(
        'items',
        'Prescription items',
        'json',
        true,
        '[{"medicineId":"","dose":"","frequency":"","duration":"","route":"","instructions":null}]',
      ),
    ],
  },
  laboratory: {
    endpoint: 'laboratory/orders',
    fields: [
      f('patientId', 'Patient ID'),
      f('doctorId', 'Doctor ID'),
      f('tests', 'Tests', 'json', true, '[{"code":"","name":"","labTestId":null}]'),
    ],
  },
  'lab-tests': {
    endpoint: 'laboratory/tests',
    fields: [
      f('name', 'Name'),
      f('code', 'Code'),
      {
        ...f('category', 'Category', 'select'),
        options: [
          'Hematology',
          'Biochemistry',
          'Microbiology',
          'Immunology',
          'Urinalysis',
          'Pathology',
          'Molecular',
        ],
      },
      f('price', 'Price', 'number'),
      f('normalRange', 'Normal range', 'text', false),
      f('unit', 'Unit', 'text', false),
    ],
  },
  radiology: {
    endpoint: 'radiology/orders',
    fields: [
      f('patientId', 'Patient ID'),
      f('doctorId', 'Doctor ID'),
      f('procedure', 'Procedure'),
      f('radiologyTestId', 'Radiology test ID', 'text', false),
    ],
  },
  'radiology-tests': {
    endpoint: 'radiology/tests',
    fields: [
      f('code', 'Code'),
      f('name', 'Name'),
      f('modality', 'Modality'),
      f('price', 'Price', 'number'),
    ],
  },
  billing: {
    endpoint: 'billing/invoices',
    fields: [
      f('patientId', 'Patient ID'),
      f(
        'items',
        'Invoice items',
        'json',
        true,
        '[{"description":"","quantity":1,"unitPrice":0,"discount":0,"tax":0}]',
      ),
    ],
  },
  admissions: {
    endpoint: 'admissions',
    fields: [
      f('patientId', 'Patient ID'),
      f('doctorId', 'Doctor ID'),
      f('admissionType', 'Admission type'),
      f('reason', 'Reason'),
      f('wardId', 'Ward ID'),
      f('roomId', 'Room ID'),
      f('bedId', 'Bed ID'),
    ],
  },
  wards: { endpoint: 'admissions/wards', fields: [f('name', 'Ward name'), f('code', 'Ward code')] },
  rooms: {
    endpoint: 'admissions/rooms',
    fields: [f('wardId', 'Ward ID'), f('number', 'Room number')],
  },
  beds: {
    endpoint: 'admissions/beds',
    fields: [f('roomId', 'Room ID'), f('number', 'Bed number')],
  },
  tenants: { endpoint: 'tenants', fields: [f('name', 'Hospital name'), f('slug', 'Tenant slug')] },
};
