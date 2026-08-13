import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormArray, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { NotificationService } from '../../../../core/http/notification.service';
import { Supplier } from '../../../suppliers/models/supplier.model';
import { SupplierService } from '../../../suppliers/services/supplier.service';
import { InventoryItem } from '../../models/inventory.model';
import { PurchaseOrder, PurchaseOrderAction } from '../../models/purchase-order.model';
import { InventoryService } from '../../services/inventory.service';
import { PurchaseOrderService } from '../../services/purchase-order.service';
@Component({
  selector: 'app-purchase-orders',
  imports: [ReactiveFormsModule, CurrencyPipe, DatePipe],
  templateUrl: './purchase-orders.component.html',
  styleUrl: './purchase-orders.component.scss',
})
export class PurchaseOrdersComponent {
  private api = inject(PurchaseOrderService);
  private inventoryApi = inject(InventoryService);
  private supplierApi = inject(SupplierService);
  private notices = inject(NotificationService);
  orders = signal<PurchaseOrder[]>([]);
  inventory = signal<InventoryItem[]>([]);
  suppliers = signal<Supplier[]>([]);
  selected = signal<PurchaseOrder | null>(null);
  modal = signal(false);
  saving = signal(false);
  loading = signal(true);
  form = new FormGroup({
    supplierId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    expectedDeliveryDate: new FormControl<string | null>(null),
    notes: new FormControl<string | null>(null),
    items: new FormArray([this.itemGroup()]),
  });
  total = computed(() =>
    this.items.controls.reduce((sum, item) => {
      const value = item.getRawValue();
      return sum + value.quantity * value.unitCost;
    }, 0),
  );
  constructor() {
    this.load();
  }
  get items() {
    return this.form.controls.items;
  }
  load() {
    this.loading.set(true);
    forkJoin({
      orders: this.api.list({ pageSize: 50 }),
      inventory: this.inventoryApi.list({ pageSize: 100 }),
      suppliers: this.supplierApi.list({ pageSize: 100 }),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe(({ orders, inventory, suppliers }) => {
        this.orders.set(orders.items);
        this.inventory.set(inventory.items.filter((item) => item.isActive));
        this.suppliers.set(suppliers.items.filter((supplier) => supplier.isActive));
      });
  }
  view(id: string) {
    this.api.get(id).subscribe((x) => this.selected.set(x));
  }
  create() {
    if (this.form.invalid) return this.form.markAllAsTouched();
    const value = this.form.getRawValue();
    this.saving.set(true);
    this.api
      .create(value)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notices.show('Purchase order created.', 'success');
        this.modal.set(false);
        this.form.reset({ supplierId: '', expectedDeliveryDate: null, notes: null });
        while (this.items.length > 1) this.items.removeAt(1);
        this.items.at(0).reset({ inventoryItemId: '', quantity: 1, unitCost: 0 });
        this.load();
      });
  }
  addItem() {
    this.items.push(this.itemGroup());
  }
  removeItem(index: number) {
    if (this.items.length > 1) this.items.removeAt(index);
  }
  supplierName(id: string) {
    return this.suppliers().find((x) => x.id === id)?.name ?? id;
  }
  itemName(id: string) {
    const item = this.inventory().find((x) => x.id === id);
    return item ? `${item.sku} — ${item.name}` : id;
  }
  action(id: string, action: PurchaseOrderAction) {
    this.api.action(id, action).subscribe((x) => {
      this.notices.show(`${action} completed.`, 'success');
      this.selected.set(x);
      this.load();
    });
  }
  private itemGroup() {
    return new FormGroup({
      inventoryItemId: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      quantity: new FormControl(1, {
        nonNullable: true,
        validators: [Validators.required, Validators.min(1)],
      }),
      unitCost: new FormControl(0, {
        nonNullable: true,
        validators: [Validators.required, Validators.min(0)],
      }),
    });
  }
}
