import { AsyncPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import {
  BehaviorSubject,
  catchError,
  combineLatest,
  debounceTime,
  finalize,
  map,
  of,
  startWith,
  switchMap,
} from 'rxjs';
import { NotificationService } from '../../core/http/notification.service';
import { AuthService } from '../../core/auth/services/auth.service';
import { ApiResource, ApiScalar } from '../../shared/models/resource.model';
import { MODULE_ACTIONS, ModuleAction } from './module-actions';
import { MODULES } from './module-config';
import { ModuleDataService } from './module-data.service';
import { MODULE_FORMS, ModuleField } from './module-forms';

type DynamicForm = FormGroup<Record<string, FormControl<string>>>;

@Component({
  selector: 'app-module-page',
  imports: [AsyncPipe, ReactiveFormsModule],
  templateUrl: './module-page.component.html',
  styleUrl: './module-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ModulePageComponent {
  readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ModuleDataService);
  private readonly notices = inject(NotificationService);
  private readonly refresh = new BehaviorSubject(0);
  private selectedId = '';
  readonly key = this.route.snapshot.routeConfig?.path ?? '';
  readonly config = MODULES[this.key] ?? {
    title: this.key,
    description: 'Hospital operations workspace.',
    search: false,
    columns: [],
  };
  readonly definition = MODULE_FORMS[this.key];
  readonly actions = MODULE_ACTIONS[this.key] ?? [];
  readonly createPermission = this.permissionForCreate(this.key);
  readonly search = new FormControl('', { nonNullable: true });

  private permissionForCreate(key: string): string {
    const permissions: Record<string, string> = {
      users: 'Users.Create',
      roles: 'Roles.Create',
      'medical-records': 'MedicalRecords.Create',
      prescriptions: 'Prescriptions.Create',
      pharmacy: 'Pharmacy.Create',
      insurance: 'Patients.Update',
      notifications: 'Users.Update',
      subscriptions: 'Tenants.Update',
      tenants: 'Tenants.Create',
      wards: 'Admissions.Create',
      rooms: 'Admissions.Create',
      beds: 'Admissions.Create',
    };
    return permissions[key] ?? '';
  }
  readonly page = signal(1);
  readonly modal = signal(false);
  readonly actionModal = signal(false);
  readonly saving = signal(false);
  readonly selectedAction = signal<ModuleAction | null>(null);
  readonly formError = signal('');
  readonly form: DynamicForm = new FormGroup<Record<string, FormControl<string>>>({});
  readonly actionForm: DynamicForm = new FormGroup<Record<string, FormControl<string>>>({});
  readonly state$ = combineLatest([
    this.search.valueChanges.pipe(startWith(''), debounceTime(250)),
    this.refresh,
  ]).pipe(
    switchMap(([search]) =>
      this.config.endpoint
        ? this.api.list(this.config.endpoint, search, this.page()).pipe(
            map((data) => ({ data, error: false })),
            catchError(() => of({ data: null, error: true })),
          )
        : of({ data: null, error: false }),
    ),
  );

  constructor() {
    this.build(this.form, this.definition?.fields ?? []);
  }
  open(): void {
    if (this.definition) {
      this.formError.set('');
      this.modal.set(true);
    }
  }
  close(): void {
    this.modal.set(false);
    this.form.reset();
    this.formError.set('');
  }
  changePage(page: number): void {
    this.page.set(Math.max(1, page));
    this.refresh.next(0);
  }
  run(action: ModuleAction, item: ApiResource): void {
    this.selectedId = String(item['id']);
    this.selectedAction.set(action);
    this.clearControls(this.actionForm);
    this.build(this.actionForm, action.fields ?? []);
    this.formError.set('');
    this.actionModal.set(true);
  }
  closeAction(): void {
    this.actionModal.set(false);
    this.selectedAction.set(null);
    this.clearControls(this.actionForm);
    this.formError.set('');
  }
  submit(): void {
    if (!this.definition || this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const payload = this.payload(this.form, this.definition.fields);
    if (!payload) return;
    this.saving.set(true);
    this.api
      .create(this.definition.endpoint, payload)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => this.done(`${this.config.title} record created successfully.`, true));
  }
  submitAction(): void {
    const action = this.selectedAction();
    if (!action || this.actionForm.invalid) {
      this.actionForm.markAllAsTouched();
      return;
    }
    const payload = this.payload(this.actionForm, action.fields ?? []);
    if (payload) this.execute(action, this.selectedId, payload);
  }
  display(value: unknown): string {
    if (Array.isArray(value))
      return value.map((v) => (typeof v === 'object' ? 'Record' : String(v))).join(', ');
    if (value === null || value === undefined || value === '') return '—';
    return String(value as ApiScalar);
  }
  fieldError(field: ModuleField, group: DynamicForm): string {
    const control = group.controls[field.key];
    if (!control.touched || !control.errors) return '';
    if (control.hasError('required')) return `${field.label} is required.`;
    if (control.hasError('email')) return 'Enter a valid email address.';
    if (control.hasError('guid')) return 'Enter a valid GUID.';
    if (control.hasError('json')) return 'Enter valid JSON.';
    if (control.hasError('number')) return 'Enter a valid number.';
    return `${field.label} is invalid.`;
  }
  private execute(action: ModuleAction, id: string, payload: Record<string, unknown>): void {
    this.saving.set(true);
    this.api
      .action(action.method, action.path(id), payload)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => this.done(`${action.label} completed successfully.`, false));
  }
  private done(message: string, created: boolean): void {
    this.notices.show(message, 'success');
    created ? this.close() : this.closeAction();
    this.refresh.next(0);
  }
  private build(group: DynamicForm, fields: ModuleField[]): void {
    for (const field of fields) {
      const validators: ValidatorFn[] = field.required ? [Validators.required] : [];
      if (field.type === 'email') validators.push(Validators.email);
      if (field.type === 'number') validators.push(this.numberValidator());
      if (field.type === 'json') validators.push(this.jsonValidator());
      if (field.type === 'guid-list') validators.push(this.guidListValidator());
      if (field.key === 'id' || field.key.endsWith('Id')) validators.push(this.guidValidator());
      group.addControl(
        field.key,
        new FormControl('', {
          nonNullable: true,
          validators,
        }),
      );
    }
  }
  private clearControls(group: DynamicForm): void {
    for (const key of Object.keys(group.controls)) group.removeControl(key as never);
  }
  private payload(group: DynamicForm, fields: ModuleField[]): Record<string, unknown> | null {
    const output: Record<string, unknown> = {};
    try {
      for (const field of fields)
        output[field.key] = this.convert(field, group.controls[field.key].value.trim());
      this.formError.set('');
      return output;
    } catch {
      this.formError.set(
        'One or more JSON fields are invalid. Check brackets, commas, and quotes.',
      );
      return null;
    }
  }
  private convert(field: ModuleField, value: string): unknown {
    if (!value) return null;
    if (field.type === 'number') return Number(value);
    if (field.type === 'boolean') return value === 'true';
    if (field.type === 'csv' || field.type === 'guid-list')
      return value
        .split(',')
        .map((item) => item.trim())
        .filter(Boolean);
    if (field.type === 'json') return JSON.parse(value) as unknown;
    if (field.type === 'datetime-local') return new Date(value).toISOString();
    return value;
  }
  private guidValidator(): ValidatorFn {
    const guid = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
    return (control: AbstractControl<string>): ValidationErrors | null =>
      !control.value || guid.test(control.value) ? null : { guid: true };
  }
  private guidListValidator(): ValidatorFn {
    const guid = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
    return (control: AbstractControl<string>): ValidationErrors | null =>
      !control.value || control.value.split(',').every((item) => guid.test(item.trim()))
        ? null
        : { guid: true };
  }
  private jsonValidator(): ValidatorFn {
    return (control: AbstractControl<string>): ValidationErrors | null => {
      if (!control.value) return null;
      try {
        JSON.parse(control.value);
        return null;
      } catch {
        return { json: true };
      }
    };
  }
  private numberValidator(): ValidatorFn {
    return (control: AbstractControl<string>): ValidationErrors | null =>
      !control.value || Number.isFinite(Number(control.value)) ? null : { number: true };
  }
}
