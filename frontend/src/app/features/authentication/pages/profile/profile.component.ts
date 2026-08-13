import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { NotificationService } from '../../../../core/http/notification.service';
import { AccountService } from '../../services/account.service';

@Component({
  selector: 'app-profile',
  imports: [ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
})
export class ProfileComponent {
  readonly auth = inject(AuthService);
  private account = inject(AccountService);
  private notices = inject(NotificationService);

  saving = signal(false);
  showCurrentPassword = signal(false);
  showNewPassword = signal(false);
  showConfirmPassword = signal(false);

  toggleCurrentPassword(): void {
    this.showCurrentPassword.update((v) => !v);
  }

  toggleNewPassword(): void {
    this.showNewPassword.update((v) => !v);
  }

  toggleConfirmPassword(): void {
    this.showConfirmPassword.update((v) => !v);
  }

  form = new FormGroup({
    currentPassword: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    newPassword: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(12)],
    }),
    confirmPassword: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  });

  copyToClipboard(text: string | undefined, label: string): void {
    if (!text) return;
    navigator.clipboard.writeText(text);
    this.notices.show(`${label} copied to clipboard!`, 'info');
  }

  submit() {
    if (this.form.invalid) return;
    const x = this.form.getRawValue();
    if (x.newPassword !== x.confirmPassword) {
      this.notices.show('Passwords do not match.', 'error');
      return;
    }
    this.saving.set(true);
    this.account
      .changePassword(x.currentPassword, x.newPassword)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notices.show('Password changed successfully. Please sign in again.', 'success');
        this.auth.clear();
      });
  }
}
