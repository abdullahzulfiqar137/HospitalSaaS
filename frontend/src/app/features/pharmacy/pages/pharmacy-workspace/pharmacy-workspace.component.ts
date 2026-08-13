import { DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, forkJoin, map, of, switchMap } from 'rxjs';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { NotificationService } from '../../../../core/http/notification.service';
import { Medicine } from '../../models/pharmacy.model';
import { PharmacyService } from '../../services/pharmacy.service';

@Component({
  selector: 'app-pharmacy-workspace',
  imports: [ReactiveFormsModule, DecimalPipe],
  templateUrl: './pharmacy-workspace.component.html',
  styleUrl: './pharmacy-workspace.component.scss',
})
export class PharmacyWorkspaceComponent {
  private readonly api = inject(PharmacyService);
  private readonly notices = inject(NotificationService);
  readonly auth = inject(AuthService);
  readonly medicines = signal<Medicine[]>([]);
  readonly balances = signal<Record<string, number>>({});
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly modal = signal<'medicine' | 'stock' | null>(null);
  readonly selected = signal<Medicine | null>(null);
  readonly search = signal('');
  readonly medicineForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: Validators.required }),
    genericName: new FormControl('', { nonNullable: true, validators: Validators.required }),
    brandName: new FormControl<string | null>(null),
    strength: new FormControl('', { nonNullable: true, validators: Validators.required }),
    dosageForm: new FormControl('', { nonNullable: true, validators: Validators.required }),
    manufacturer: new FormControl<string | null>(null),
    barcode: new FormControl<string | null>(null),
    salePrice: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0)],
    }),
    purchasePrice: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0)],
    }),
    minimumStockLevel: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0)],
    }),
  });
  readonly stockForm = new FormGroup({
    type: new FormControl<'Purchase' | 'Sale' | 'Adjustment' | 'Return'>('Purchase', {
      nonNullable: true,
    }),
    quantity: new FormControl(1, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(1)],
    }),
    reference: new FormControl<string | null>(null),
  });
  constructor() {
    this.load();
  }
  load(): void {
    this.loading.set(true);
    this.api
      .list({ search: this.search(), pageSize: 100 })
      .pipe(
        switchMap((page) =>
          page.items.length
            ? forkJoin(
                page.items.map((item) =>
                  this.api.balance(item.id).pipe(map((x) => [item.id, x.quantity] as const)),
                ),
              ).pipe(
                map((entries) => ({ items: page.items, balances: Object.fromEntries(entries) })),
              )
            : of({ items: page.items, balances: {} }),
        ),
        finalize(() => this.loading.set(false)),
      )
      .subscribe((data) => {
        this.medicines.set(data.items);
        this.balances.set(data.balances);
      });
  }
  saveMedicine(): void {
    if (this.medicineForm.invalid) return this.medicineForm.markAllAsTouched();
    this.saving.set(true);
    this.api
      .createMedicine(this.medicineForm.getRawValue())
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notices.show('Medicine added.', 'success');
        this.modal.set(null);
        this.medicineForm.reset({ salePrice: 0, purchasePrice: 0, minimumStockLevel: 0 } as never);
        this.load();
      });
  }
  openStock(item: Medicine): void {
    this.selected.set(item);
    this.stockForm.reset({ type: 'Purchase', quantity: 1, reference: null });
    this.modal.set('stock');
  }
  saveStock(): void {
    if (this.stockForm.invalid || !this.selected()) return;
    this.saving.set(true);
    this.api
      .stock(this.selected()!.id, this.stockForm.getRawValue())
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notices.show('Stock transaction recorded.', 'success');
        this.modal.set(null);
        this.load();
      });
  }
  remove(item: Medicine): void {
    if (confirm(`Delete ${item.name}?`))
      this.api.delete(item.id).subscribe(() => {
        this.notices.show('Medicine deleted.', 'success');
        this.load();
      });
  }
  lowStock(item: Medicine): boolean {
    return this.balance(item.id) <= item.minimumStockLevel;
  }
  balance(id: string): number {
    return this.balances()[id] || 0;
  }
  lowStockCount(): number {
    return this.medicines().filter((item) => this.lowStock(item)).length;
  }
}
