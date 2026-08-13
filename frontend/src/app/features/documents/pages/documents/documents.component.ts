import { DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { NotificationService } from '../../../../core/http/notification.service';
import { ConfirmationDialogComponent } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { HospitalDocument } from '../../models/document.model';
import { DocumentService } from '../../services/document.service';
@Component({
  selector: 'app-documents',
  imports: [ReactiveFormsModule, DecimalPipe, ConfirmationDialogComponent],
  templateUrl: './documents.component.html',
  styleUrl: './documents.component.scss',
})
export class DocumentsComponent {
  private api = inject(DocumentService);
  private notices = inject(NotificationService);
  documents = signal<HospitalDocument[]>([]);
  loading = signal(false);
  uploading = signal(false);
  file = signal<File | null>(null);
  pendingDelete = signal<string | null>(null);
  deleting = signal(false);
  filter = new FormControl('', { nonNullable: true });
  patientId = new FormControl('', { nonNullable: true, validators: [Validators.required] });
  constructor() {
    this.load();
  }
  load() {
    this.loading.set(true);
    this.api
      .list(this.filter.value)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe((x) => this.documents.set(x.items));
  }
  choose(e: Event) {
    this.file.set((e.target as HTMLInputElement).files?.[0] ?? null);
  }
  upload() {
    const file = this.file();
    if (!file || this.patientId.invalid) return;
    if (file.size > 20000000) {
      this.notices.show('Maximum file size is 20 MB.', 'error');
      return;
    }
    this.uploading.set(true);
    this.api
      .upload(this.patientId.value, file)
      .pipe(finalize(() => this.uploading.set(false)))
      .subscribe(() => {
        this.notices.show('Document uploaded.', 'success');
        this.file.set(null);
        this.load();
      });
  }
  remove(id: string) {
    this.pendingDelete.set(id);
  }
  confirmDelete() {
    const id = this.pendingDelete();
    if (!id) return;
    this.deleting.set(true);
    this.api
      .delete(id)
      .pipe(finalize(() => this.deleting.set(false)))
      .subscribe(() => {
        this.notices.show('Document deleted.', 'success');
        this.pendingDelete.set(null);
        this.load();
      });
  }
}
