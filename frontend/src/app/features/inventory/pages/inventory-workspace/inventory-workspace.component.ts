import { Component, computed, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { catchError, finalize, forkJoin, map, of } from 'rxjs';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { NotificationService } from '../../../../core/http/notification.service';
import { ConfirmationDialogComponent } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { Supplier } from '../../../suppliers/models/supplier.model';
import { SupplierService } from '../../../suppliers/services/supplier.service';
import { InventoryItem } from '../../models/inventory.model';
import { InventoryService } from '../../services/inventory.service';

@Component({
  selector: 'app-inventory-workspace',
  imports: [ReactiveFormsModule, RouterLink, ConfirmationDialogComponent],
  templateUrl: './inventory-workspace.component.html',
  styleUrl: './inventory-workspace.component.scss',
})
export class InventoryWorkspaceComponent {
  private readonly inventoryApi = inject(InventoryService);
  private readonly supplierApi = inject(SupplierService);
  private readonly notices = inject(NotificationService);
  readonly auth = inject(AuthService);
  readonly tab = signal<'inventory' | 'suppliers'>(
    location.pathname.endsWith('/suppliers') ? 'suppliers' : 'inventory',
  );
  readonly inventory = signal<InventoryItem[]>([]);
  readonly suppliers = signal<Supplier[]>([]);
  readonly stocks = signal<Partial<Record<string, number>>>({});
  readonly search = signal('');
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly itemOpen = signal(false);
  readonly supplierOpen = signal(false);
  readonly adjustmentOpen = signal(false);
  readonly selectedItem = signal<InventoryItem | null>(null);
  readonly pendingDelete = signal<{ type: 'item' | 'supplier'; id: string; name: string } | null>(
    null,
  );
  readonly filteredItems = computed(() => this.filter(this.inventory(), ['sku', 'name', 'unit']));
  readonly filteredSuppliers = computed(() =>
    this.filter(this.suppliers(), ['name', 'contactName', 'phone', 'email']),
  );
  readonly lowStockCount = computed(
    () =>
      this.inventory().filter((item) => (this.stocks()[item.id] ?? 0) <= item.minimumStock).length,
  );

  readonly itemForm = new FormGroup({
    sku: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    name: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    unit: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    minimumStock: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0)],
    }),
  });
  readonly supplierForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    contactName: new FormControl<string | null>(null),
    phone: new FormControl<string | null>(null),
    email: new FormControl<string | null>(null, Validators.email),
    address: new FormControl<string | null>(null),
  });
  readonly adjustmentForm = new FormGroup({
    quantity: new FormControl(0, { nonNullable: true, validators: [Validators.required] }),
    reason: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  });

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    forkJoin({
      inventory: this.inventoryApi.list({ pageSize: 100 }),
      suppliers: this.supplierApi.list({ pageSize: 100 }),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe(({ inventory, suppliers }) => {
        this.inventory.set(inventory.items);
        this.suppliers.set(suppliers.items);
        this.loadStocks(inventory.items);
      });
  }

  createItem(): void {
    if (this.itemForm.invalid) return this.itemForm.markAllAsTouched();
    this.saving.set(true);
    this.inventoryApi
      .createItem(this.itemForm.getRawValue())
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notices.show('Inventory item created.', 'success');
        this.itemOpen.set(false);
        this.itemForm.reset({ sku: '', name: '', unit: '', minimumStock: 0 });
        this.load();
      });
  }

  createSupplier(): void {
    if (this.supplierForm.invalid) return this.supplierForm.markAllAsTouched();
    this.saving.set(true);
    this.supplierApi
      .createSupplier(this.supplierForm.getRawValue())
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notices.show('Supplier created.', 'success');
        this.supplierOpen.set(false);
        this.supplierForm.reset();
        this.load();
      });
  }

  openAdjustment(item: InventoryItem): void {
    this.selectedItem.set(item);
    this.adjustmentForm.reset({ quantity: 0, reason: '' });
    this.adjustmentOpen.set(true);
  }

  adjust(): void {
    const item = this.selectedItem();
    if (!item || this.adjustmentForm.invalid || this.adjustmentForm.controls.quantity.value === 0)
      return;
    this.saving.set(true);
    this.inventoryApi
      .adjust(item.id, this.adjustmentForm.getRawValue())
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notices.show('Stock balance updated.', 'success');
        this.adjustmentOpen.set(false);
        this.load();
      });
  }

  confirmDelete(): void {
    const target = this.pendingDelete();
    if (!target) return;
    const request =
      target.type === 'item'
        ? this.inventoryApi.delete(target.id)
        : this.supplierApi.delete(target.id);
    this.saving.set(true);
    request.pipe(finalize(() => this.saving.set(false))).subscribe(() => {
      this.notices.show(`${target.name} removed.`, 'success');
      this.pendingDelete.set(null);
      this.load();
    });
  }

  private loadStocks(items: InventoryItem[]): void {
    if (!items.length) return this.stocks.set({});
    forkJoin(
      items.map((item) =>
        this.inventoryApi.stock(item.id).pipe(
          map((stock) => [item.id, stock.quantity] as const),
          catchError(() => of([item.id, 0] as const)),
        ),
      ),
    ).subscribe((entries) => this.stocks.set(Object.fromEntries(entries)));
  }

  private filter<T>(items: T[], keys: Array<keyof T>): T[] {
    const value = this.search().trim().toLowerCase();
    return value
      ? items.filter((item) =>
          keys.some((key) =>
            String(item[key] ?? '')
              .toLowerCase()
              .includes(value),
          ),
        )
      : items;
  }
}
