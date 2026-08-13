import { Injectable, signal } from '@angular/core';
export interface Toast {
  message: string;
  tone: 'success' | 'error' | 'info';
}
@Injectable({ providedIn: 'root' })
export class NotificationService {
  readonly toast = signal<Toast | null>(null);
  show(message: string, tone: Toast['tone'] = 'info'): void {
    this.toast.set({ message, tone });
    window.setTimeout(() => this.toast.set(null), 4500);
  }
}
