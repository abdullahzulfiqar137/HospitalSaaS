import { ModuleField } from './module-forms';
export interface ModuleAction {
  label: string;
  method: 'post' | 'delete';
  path: (id: string) => string;
  fields?: ModuleField[];
  confirm?: string;
  permission?: string;
}
const f = (
  key: string,
  label: string,
  type: ModuleField['type'] = 'text',
  required = true,
): ModuleField => ({ key, label, type, required });
export const MODULE_ACTIONS: Record<string, ModuleAction[]> = {
  doctors: [
    { label: 'Activate', method: 'post', path: (id) => `doctors/${id}/activate` },
    { label: 'Deactivate', method: 'post', path: (id) => `doctors/${id}/deactivate` },
    {
      label: 'Delete',
      method: 'delete',
      path: (id) => `doctors/${id}`,
      confirm: 'Delete this doctor?',
    },
  ],
  departments: [
    {
      label: 'Delete',
      method: 'delete',
      path: (id) => `departments/${id}`,
      confirm: 'Delete this department?',
    },
  ],
  appointments: [
    {
      label: 'Update status',
      method: 'post',
      path: (id) => `appointments/${id}/actions`,
      fields: [
        f('action', 'Action'),
        f('reason', 'Reason', 'textarea', false),
        f('appointmentDateTimeUtc', 'New time', 'datetime-local', false),
        f('durationMinutes', 'Duration', 'number', false),
      ],
    },
  ],
  pharmacy: [
    {
      label: 'Stock',
      permission: 'Pharmacy.Dispense',
      method: 'post',
      path: (id) => `pharmacy/medicines/${id}/stock`,
      fields: [
        { ...f('type', 'Type', 'select'), options: ['Purchase', 'Sale', 'Adjustment', 'Return'] },
        f('quantity', 'Quantity', 'number'),
        f('reference', 'Reference', 'text', false),
      ],
    },
    {
      label: 'Delete',
      permission: 'Pharmacy.Create',
      method: 'delete',
      path: (id) => `pharmacy/medicines/${id}`,
      confirm: 'Delete this medicine?',
    },
  ],
  laboratory: [
    {
      label: 'Order action',
      method: 'post',
      path: (id) => `laboratory/orders/${id}/actions`,
      fields: [f('action', 'Action')],
    },
    {
      label: 'Record result',
      method: 'post',
      path: (id) => `laboratory/orders/${id}/results`,
      fields: [
        f('itemId', 'Order item ID'),
        f('result', 'Result'),
        f('unit', 'Unit', 'text', false),
        f('referenceRange', 'Reference range', 'text', false),
      ],
    },
  ],
  radiology: [
    {
      label: 'Record report',
      method: 'post',
      path: (id) => `radiology/orders/${id}/report`,
      fields: [f('report', 'Report', 'textarea')],
    },
  ],
  billing: [
    {
      label: 'Record payment',
      method: 'post',
      path: (id) => `billing/invoices/${id}/payments`,
      fields: [f('amount', 'Amount', 'number'), f('method', 'Method')],
    },
  ],
  payments: [
    {
      label: 'Refund',
      method: 'post',
      path: (id) => `billing/payments/${id}/refund`,
      fields: [f('amount', 'Refund amount', 'number')],
    },
  ],
  inventory: [
    {
      label: 'Adjust stock',
      method: 'post',
      path: (id) => `inventory/${id}/adjustments`,
      fields: [f('quantity', 'Quantity', 'number'), f('reason', 'Reason')],
    },
    {
      label: 'Delete',
      method: 'delete',
      path: (id) => `inventory/${id}`,
      confirm: 'Delete this inventory item?',
    },
  ],
  suppliers: [
    {
      label: 'Delete',
      method: 'delete',
      path: (id) => `suppliers/${id}`,
      confirm: 'Delete this supplier?',
    },
  ],
  'medical-records': [
    {
      label: 'Add diagnosis',
      permission: 'MedicalRecords.Create',
      method: 'post',
      path: (id) => `medical-records/${id}/diagnoses`,
      fields: [
        f('code', 'Diagnosis code'),
        f('description', 'Description', 'textarea'),
        { ...f('isPrimary', 'Primary diagnosis', 'boolean'), options: ['true', 'false'] },
      ],
    },
    {
      label: 'Delete',
      permission: 'MedicalRecords.Create',
      method: 'delete',
      path: (id) => `medical-records/${id}`,
      confirm: 'Delete this record?',
    },
  ],
  documents: [
    {
      label: 'Delete',
      method: 'delete',
      path: (id) => `documents/${id}`,
      confirm: 'Delete this document?',
    },
  ],
};
