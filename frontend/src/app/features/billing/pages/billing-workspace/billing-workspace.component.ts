import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormArray, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { catchError, finalize, forkJoin, of } from 'rxjs';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { NotificationService } from '../../../../core/http/notification.service';
import { Patient } from '../../../patients/models/patient.model';
import { PatientService } from '../../../patients/services/patient.service';
import { Payment, Invoice } from '../../models/billing.model';
import { BillingService } from '../../services/billing.service';
import { PaymentService } from '../../../payments/services/payment.service';

@Component({
  selector: 'app-billing-workspace',
  imports: [ReactiveFormsModule, CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './billing-workspace.component.html',
  styleUrl: './billing-workspace.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BillingWorkspaceComponent {
  private readonly billing = inject(BillingService);
  private readonly paymentsApi = inject(PaymentService);
  private readonly patientsApi = inject(PatientService);
  private readonly notices = inject(NotificationService);
  readonly auth = inject(AuthService);
  readonly route = inject(ActivatedRoute);

  readonly tab = signal<'invoices' | 'payments'>(
    location.pathname.endsWith('/payments') ? 'payments' : 'invoices',
  );
  readonly invoices = signal<Invoice[]>([]);
  readonly payments = signal<Payment[]>([]);
  readonly patients = signal<Patient[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly invoiceOpen = signal(false);
  readonly paymentOpen = signal(false);
  readonly refundOpen = signal(false);
  readonly selectedInvoice = signal<Invoice | null>(null);
  readonly selectedPayment = signal<Payment | null>(null);
  readonly invoicePage = signal(1);
  readonly paymentPage = signal(1);
  readonly invoicePages = signal(1);
  readonly paymentPages = signal(1);
  readonly visibleInvoiceTotal = computed(() =>
    this.invoices().reduce((total, invoice) => total + invoice.total, 0),
  );

  readonly invoiceForm = new FormGroup({
    patientId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    items: new FormArray([this.lineGroup()]),
  });
  readonly paymentForm = new FormGroup({
    amount: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0.01)],
    }),
    method: new FormControl('Card', { nonNullable: true, validators: [Validators.required] }),
  });
  readonly refundForm = new FormGroup({
    amount: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0.01)],
    }),
  });
  readonly draftTotal = computed(() =>
    this.items.controls.reduce((total, control) => {
      const item = control.getRawValue();
      return total + item.quantity * item.unitPrice - item.discount + item.tax;
    }, 0),
  );

  constructor() {
    this.load();
  }

  get items() {
    return this.invoiceForm.controls.items;
  }

  load(): void {
    this.loading.set(true);
    const fallbackPaged = { items: [], pageNumber: 1, pageSize: 15, totalCount: 0, totalPages: 1, hasPreviousPage: false, hasNextPage: false };
    forkJoin({
      invoices: this.billing.list({ pageNumber: this.invoicePage(), pageSize: 15 }).pipe(catchError(() => of(fallbackPaged))),
      payments: this.paymentsApi.list({ pageNumber: this.paymentPage(), pageSize: 15 }).pipe(catchError(() => of(fallbackPaged))),
      patients: this.patientsApi.list({ pageNumber: 1, pageSize: 100 }).pipe(catchError(() => of({ ...fallbackPaged, pageSize: 100 }))),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe(({ invoices, payments, patients }) => {
        this.invoices.set(invoices.items || []);
        this.payments.set(payments.items || []);
        this.patients.set(patients.items || []);
        this.invoicePages.set(invoices.totalPages || 1);
        this.paymentPages.set(payments.totalPages || 1);
      });
  }

  addLine(): void {
    this.items.push(this.lineGroup());
  }

  removeLine(index: number): void {
    if (this.items.length > 1) this.items.removeAt(index);
  }

  createInvoice(): void {
    if (this.invoiceForm.invalid) return this.invoiceForm.markAllAsTouched();
    this.saving.set(true);
    this.billing
      .create(this.invoiceForm.getRawValue())
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe((invoice) => {
        this.notices.show(`Invoice ${invoice.invoiceNumber} created.`, 'success');
        this.invoiceOpen.set(false);
        this.invoiceForm.reset({ patientId: '' });
        while (this.items.length > 1) this.items.removeAt(1);
        this.items.at(0).reset({ description: '', quantity: 1, unitPrice: 0, discount: 0, tax: 0 });
        this.load();
      });
  }

  formatPatientLabel(patient: { firstName: string; lastName: string; patientNumber: string }): string {
    const cleanLast = (patient.lastName || '').replace(/\d+/g, '').trim();
    const name = `${patient.firstName} ${cleanLast}`.trim();
    return `${name} (${patient.patientNumber})`;
  }

  openPayment(invoice: Invoice): void {
    this.selectedInvoice.set(invoice);
    this.paymentForm.reset({ amount: invoice.total, method: 'Card' });
    this.paymentOpen.set(true);
  }

  recordPayment(): void {
    const invoice = this.selectedInvoice();
    if (!invoice || this.paymentForm.invalid) return;
    this.saving.set(true);
    this.billing
      .recordPayment(invoice.id, this.paymentForm.getRawValue())
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notices.show('Payment recorded successfully.', 'success');
        this.paymentOpen.set(false);
        this.load();
      });
  }

  openRefund(payment: Payment): void {
    this.selectedPayment.set(payment);
    this.refundForm.reset({ amount: payment.amount - payment.refundedAmount });
    this.refundOpen.set(true);
  }

  refund(): void {
    const payment = this.selectedPayment();
    if (!payment || this.refundForm.invalid) return;
    this.saving.set(true);
    this.paymentsApi
      .refund(payment.id, this.refundForm.getRawValue())
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notices.show('Refund completed.', 'success');
        this.refundOpen.set(false);
        this.load();
      });
  }

  patientName(id: string): string {
    const patient = this.patients().find((item) => item.id === id);
    return patient ? `${patient.firstName} ${patient.lastName}` : id;
  }

  changePage(kind: 'invoice' | 'payment', page: number): void {
    if (kind === 'invoice') this.invoicePage.set(page);
    else this.paymentPage.set(page);
    this.load();
  }

  private lineGroup() {
    return new FormGroup({
      description: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
      quantity: new FormControl(1, {
        nonNullable: true,
        validators: [Validators.required, Validators.min(0.01)],
      }),
      unitPrice: new FormControl(0, {
        nonNullable: true,
        validators: [Validators.required, Validators.min(0)],
      }),
      discount: new FormControl(0, { nonNullable: true, validators: [Validators.min(0)] }),
      tax: new FormControl(0, { nonNullable: true, validators: [Validators.min(0)] }),
    });
  }
}
