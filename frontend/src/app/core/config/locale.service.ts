import { DOCUMENT } from '@angular/common';
import { Injectable, inject, signal } from '@angular/core';

export interface AppLocale {
  code: 'en' | 'ur' | 'ar';
  label: string;
  direction: 'ltr' | 'rtl';
}

@Injectable({ providedIn: 'root' })
export class LocaleService {
  private readonly document = inject(DOCUMENT);
  private readonly storageKey = 'hospital.locale';
  readonly locales: AppLocale[] = [
    { code: 'en', label: 'English', direction: 'ltr' },
    { code: 'ur', label: 'اردو', direction: 'rtl' },
    { code: 'ar', label: 'العربية', direction: 'rtl' },
  ];
  private readonly selectedState = signal<AppLocale>(
    this.resolve(localStorage.getItem(this.storageKey)),
  );
  readonly selected = this.selectedState.asReadonly();

  constructor() {
    this.apply(this.selectedState());
  }

  set(code: string): void {
    const locale = this.resolve(code);
    localStorage.setItem(this.storageKey, locale.code);
    this.selectedState.set(locale);
    this.apply(locale);
  }

  private resolve(code: string | null): AppLocale {
    return this.locales.find((locale) => locale.code === code) ?? this.locales[0];
  }

  private apply(locale: AppLocale): void {
    this.document.documentElement.lang = locale.code;
    this.document.documentElement.dir = locale.direction;
  }
}
