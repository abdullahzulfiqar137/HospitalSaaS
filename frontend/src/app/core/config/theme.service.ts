import { Injectable, signal } from '@angular/core';
@Injectable({ providedIn: 'root' })
export class ThemeService {
  readonly dark = signal(localStorage.getItem('hospital.theme') === 'dark');
  constructor() {
    this.apply();
  }
  toggle() {
    this.dark.update((x) => !x);
    localStorage.setItem('hospital.theme', this.dark() ? 'dark' : 'light');
    this.apply();
  }
  private apply() {
    document.documentElement.classList.toggle('dark', this.dark());
  }
}
