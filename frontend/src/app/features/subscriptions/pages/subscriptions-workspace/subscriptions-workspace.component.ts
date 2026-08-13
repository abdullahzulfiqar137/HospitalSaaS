import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { NotificationService } from '../../../../core/http/notification.service';

export interface PlanTier {
  id: string;
  name: string;
  badge: string;
  monthlyPrice: number;
  annualPrice: number;
  description: string;
  isPopular?: boolean;
  isCurrent?: boolean;
  features: string[];
}

export interface InvoiceRecord {
  id: string;
  invoiceNo: string;
  date: string;
  amount: number;
  status: 'Paid' | 'Pending';
  method: string;
}

@Component({
  selector: 'app-subscriptions-workspace',
  imports: [CurrencyPipe, DatePipe],
  templateUrl: './subscriptions-workspace.component.html',
  styleUrl: './subscriptions-workspace.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SubscriptionsWorkspaceComponent {
  readonly auth = inject(AuthService);
  private readonly notices = inject(NotificationService);

  readonly isAnnual = signal(true);
  readonly upgradingPlan = signal<string | null>(null);

  readonly plans: PlanTier[] = [
    {
      id: 'starter',
      name: 'Starter Clinic',
      badge: 'SINGLE OPD FACILITY',
      monthlyPrice: 299,
      annualPrice: 249,
      description: 'Ideal for small private clinics and single-doctor practices.',
      features: [
        'Up to 5 Doctor Seats',
        '1,000 Active Patient Records',
        'OPD Appointment Scheduling',
        'Basic Digital Prescriptions',
        'Standard Email Support',
      ],
    },
    {
      id: 'enterprise',
      name: 'Enterprise Hospital',
      badge: 'CURRENT ACTIVE PLAN',
      monthlyPrice: 499,
      annualPrice: 399,
      isPopular: true,
      isCurrent: true,
      description: 'Full-suite hospital operations for multispecialty centers.',
      features: [
        'Unlimited Doctors & Clinical Staff',
        'All 7 RBAC Portals (Admin, Doctor, Lab, etc.)',
        'Pharmacy Stock & Purchase Orders',
        'Radiology & Diagnostic Orders',
        'Dedicated Multi-Tenant Database',
        '24/7 Priority SLA Support',
      ],
    },
    {
      id: 'network',
      name: 'Ultimate Network',
      badge: 'REGIONAL HEALTH CHAINS',
      monthlyPrice: 899,
      annualPrice: 749,
      description: 'Custom infrastructure for hospital networks and state health chains.',
      features: [
        'Multi-Hospital Facility Nodes',
        'HL7 / FHIR Interoperability API',
        'Custom Data Residency & On-Premises Option',
        'Dedicated Solutions Architect',
        'HIPAA & ISO 27001 Compliance Shield',
      ],
    },
  ];

  readonly invoices: InvoiceRecord[] = [
    {
      id: 'inv-1',
      invoiceNo: 'INV-2026-001',
      date: '2026-01-01',
      amount: 4788,
      status: 'Paid',
      method: 'Visa Ending in •••• 8849',
    },
    {
      id: 'inv-2',
      invoiceNo: 'INV-2025-012',
      date: '2025-01-01',
      amount: 4788,
      status: 'Paid',
      method: 'Mastercard Ending in •••• 4120',
    },
    {
      id: 'inv-3',
      invoiceNo: 'INV-2024-012',
      date: '2024-01-01',
      amount: 3588,
      status: 'Paid',
      method: 'Corporate Wire Transfer',
    },
  ];

  toggleBillingCycle(): void {
    this.isAnnual.update((v) => !v);
  }

  selectPlan(plan: PlanTier): void {
    if (plan.isCurrent) {
      this.notices.show('You are currently on the Enterprise Hospital Plan.', 'info');
      return;
    }
    this.upgradingPlan.set(plan.id);
    setTimeout(() => {
      this.upgradingPlan.set(null);
      this.notices.show(`Upgrade request for ${plan.name} submitted successfully!`, 'success');
    }, 1000);
  }

  downloadInvoice(invoice: InvoiceRecord): void {
    this.notices.show(`Downloading PDF Receipt for ${invoice.invoiceNo}...`, 'info');
  }
}
