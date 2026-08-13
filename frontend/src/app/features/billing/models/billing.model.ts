export interface InvoiceItemRequest {
  description: string;
  quantity: number;
  unitPrice: number;
  discount: number;
  tax: number;
}

export interface CreateInvoiceRequest {
  patientId: string;
  items: InvoiceItemRequest[];
}

export interface InvoiceLine extends InvoiceItemRequest {
  id: string;
}

export interface Invoice {
  id: string;
  invoiceNumber: string;
  patientId: string;
  issuedAtUtc: string;
  status: string;
  subtotal: number;
  discount: number;
  tax: number;
  total: number;
  items: InvoiceLine[];
}

export interface InvoiceQuery {
  patientId?: string;
  status?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface RecordPaymentRequest {
  amount: number;
  method: string;
}

export interface RefundPaymentRequest {
  amount: number;
}

export interface Payment {
  id: string;
  invoiceId: string;
  paymentNumber: string;
  amount: number;
  method: string;
  paidAtUtc: string;
  status: string;
  refundedAmount: number;
}

export interface PaymentQuery {
  invoiceId?: string;
  pageNumber?: number;
  pageSize?: number;
}
