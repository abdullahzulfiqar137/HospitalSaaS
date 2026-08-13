import { CommonModule } from '@angular/common';
import { Component, ElementRef, HostListener, EventEmitter, Input, Output, signal } from '@angular/core';

export interface Country {
  iso: string;
  code: string;
  name: string;
  idCardName: string;
  idExample: string;
  idMaxLength: number;
  phoneExample: string;
  phoneMaxLength: number;
}

export const COUNTRIES: Country[] = [
  { iso: 'pk', code: '+92', name: 'Pakistan', idCardName: 'CNIC', idExample: 'e.g. 42101-1234567-1', idMaxLength: 15, phoneExample: '300 1234567', phoneMaxLength: 11 },
  { iso: 'us', code: '+1', name: 'United States', idCardName: 'SSN / Driver License', idExample: 'e.g. 123-45-6789', idMaxLength: 11, phoneExample: '(555) 019-2834', phoneMaxLength: 14 },
  { iso: 'gb', code: '+44', name: 'United Kingdom', idCardName: 'NIN / NHS Number', idExample: 'e.g. QQ 12 34 56 A', idMaxLength: 13, phoneExample: '7911 123456', phoneMaxLength: 12 },
  { iso: 'in', code: '+91', name: 'India', idCardName: 'Aadhaar Card', idExample: 'e.g. 1234-5678-9012', idMaxLength: 14, phoneExample: '98765 43210', phoneMaxLength: 11 },
  { iso: 'ae', code: '+971', name: 'UAE', idCardName: 'Emirates ID', idExample: 'e.g. 784-1990-1234567-1', idMaxLength: 18, phoneExample: '50 123 4567', phoneMaxLength: 11 },
  { iso: 'sa', code: '+966', name: 'Saudi Arabia', idCardName: 'Iqama / National ID', idExample: 'e.g. 1023456789', idMaxLength: 10, phoneExample: '50 123 4567', phoneMaxLength: 11 },
  { iso: 'de', code: '+49', name: 'Germany', idCardName: 'Personalausweis', idExample: 'e.g. L01X00T47', idMaxLength: 10, phoneExample: '151 23456789', phoneMaxLength: 12 },
  { iso: 'fr', code: '+33', name: 'France', idCardName: 'Carte d Identity', idExample: 'e.g. 1 85 05 78 006 084 23', idMaxLength: 18, phoneExample: '6 12 34 56 78', phoneMaxLength: 13 },
  { iso: 'au', code: '+61', name: 'Australia', idCardName: 'Medicare / Driver License', idExample: 'e.g. 2123 45678 1', idMaxLength: 12, phoneExample: '412 345 678', phoneMaxLength: 11 },
  { iso: 'jp', code: '+81', name: 'Japan', idCardName: 'My Number Card', idExample: 'e.g. 1234 5678 9012', idMaxLength: 14, phoneExample: '90 1234 5678', phoneMaxLength: 12 },
  { iso: 'cn', code: '+86', name: 'China', idCardName: 'Resident Identity Card', idExample: 'e.g. 110101199003072345', idMaxLength: 18, phoneExample: '139 1234 5678', phoneMaxLength: 13 },
  { iso: 'tr', code: '+90', name: 'Turkey', idCardName: 'Kimlik Card', idExample: 'e.g. 12345678901', idMaxLength: 11, phoneExample: '501 123 4567', phoneMaxLength: 12 },
  { iso: 'my', code: '+60', name: 'Malaysia', idCardName: 'MyKad', idExample: 'e.g. 900101-14-5678', idMaxLength: 14, phoneExample: '12-345 6789', phoneMaxLength: 11 },
  { iso: 'id', code: '+62', name: 'Indonesia', idCardName: 'KTP Card', idExample: 'e.g. 3171012304900001', idMaxLength: 16, phoneExample: '812-3456-7890', phoneMaxLength: 13 },
  { iso: 'eg', code: '+20', name: 'Egypt', idCardName: 'National ID Card', idExample: 'e.g. 29001011234567', idMaxLength: 14, phoneExample: '100 123 4567', phoneMaxLength: 11 },
  { iso: 'za', code: '+27', name: 'South Africa', idCardName: 'Smart ID Card', idExample: 'e.g. 900101 5009 08 7', idMaxLength: 15, phoneExample: '82 123 4567', phoneMaxLength: 11 },
  { iso: 'br', code: '+55', name: 'Brazil', idCardName: 'CPF Card', idExample: 'e.g. 123.456.789-00', idMaxLength: 14, phoneExample: '11 91234-5678', phoneMaxLength: 13 },
  { iso: 'it', code: '+39', name: 'Italy', idCardName: 'Carta d Identita', idExample: 'e.g. CA12345AA', idMaxLength: 9, phoneExample: '312 345 6789', phoneMaxLength: 12 },
  { iso: 'es', code: '+34', name: 'Spain', idCardName: 'DNI Card', idExample: 'e.g. 12345678Z', idMaxLength: 9, phoneExample: '612 34 56 78', phoneMaxLength: 12 },
  { iso: 'ru', code: '+7', name: 'Russia', idCardName: 'Internal Passport', idExample: 'e.g. 4509 123456', idMaxLength: 11, phoneExample: '912 345-67-89', phoneMaxLength: 13 },
];

export function formatNationalId(iso: string, raw: string): string {
  if (!raw) return '';
  const digits = raw.replace(/\D/g, '');

  if (iso === 'pk') {
    // 5 - 7 - 1 (Max 13 digits) -> 42101-1234567-1
    const clean = digits.slice(0, 13);
    if (clean.length <= 5) return clean;
    if (clean.length <= 12) return `${clean.slice(0, 5)}-${clean.slice(5)}`;
    return `${clean.slice(0, 5)}-${clean.slice(5, 12)}-${clean.slice(12)}`;
  }

  if (iso === 'us') {
    // 3 - 2 - 4 (Max 9 digits) -> 123-45-6789
    const clean = digits.slice(0, 9);
    if (clean.length <= 3) return clean;
    if (clean.length <= 5) return `${clean.slice(0, 3)}-${clean.slice(3)}`;
    return `${clean.slice(0, 3)}-${clean.slice(3, 5)}-${clean.slice(5)}`;
  }

  if (iso === 'ae') {
    // 3 - 4 - 7 - 1 (Max 15 digits) -> 784-1990-1234567-1
    const clean = digits.slice(0, 15);
    if (clean.length <= 3) return clean;
    if (clean.length <= 7) return `${clean.slice(0, 3)}-${clean.slice(3)}`;
    if (clean.length <= 14) return `${clean.slice(0, 3)}-${clean.slice(3, 7)}-${clean.slice(7)}`;
    return `${clean.slice(0, 3)}-${clean.slice(3, 7)}-${clean.slice(7, 14)}-${clean.slice(14)}`;
  }

  if (iso === 'in') {
    // 4 - 4 - 4 (Max 12 digits) -> 1234-5678-9012
    const clean = digits.slice(0, 12);
    if (clean.length <= 4) return clean;
    if (clean.length <= 8) return `${clean.slice(0, 4)}-${clean.slice(4)}`;
    return `${clean.slice(0, 4)}-${clean.slice(4, 8)}-${clean.slice(8)}`;
  }

  if (iso === 'my') {
    // 6 - 2 - 4 (Max 12 digits) -> 900101-14-5678
    const clean = digits.slice(0, 12);
    if (clean.length <= 6) return clean;
    if (clean.length <= 8) return `${clean.slice(0, 6)}-${clean.slice(6)}`;
    return `${clean.slice(0, 6)}-${clean.slice(6, 8)}-${clean.slice(8)}`;
  }

  if (iso === 'br') {
    // 3 . 3 . 3 - 2 (Max 11 digits) -> 123.456.789-00
    const clean = digits.slice(0, 11);
    if (clean.length <= 3) return clean;
    if (clean.length <= 6) return `${clean.slice(0, 3)}.${clean.slice(3)}`;
    if (clean.length <= 9) return `${clean.slice(0, 3)}.${clean.slice(3, 6)}.${clean.slice(6)}`;
    return `${clean.slice(0, 3)}.${clean.slice(3, 6)}.${clean.slice(6, 9)}-${clean.slice(9)}`;
  }

  if (iso === 'sa') return digits.slice(0, 10);
  if (iso === 'tr') return digits.slice(0, 11);
  if (iso === 'eg') return digits.slice(0, 14);
  if (iso === 'id') return digits.slice(0, 16);
  if (iso === 'cn') return digits.slice(0, 18);

  return raw.slice(0, 20);
}

export function formatPhoneNumber(iso: string, raw: string): string {
  if (!raw) return '';
  const digits = raw.replace(/\D/g, '');
  if (iso === 'pk') {
    // 300 1234567 (10 digits)
    const clean = digits.slice(0, 10);
    if (clean.length <= 3) return clean;
    return `${clean.slice(0, 3)} ${clean.slice(3)}`;
  }
  if (iso === 'us') {
    // (555) 019-2834 (10 digits)
    const clean = digits.slice(0, 10);
    if (clean.length <= 3) return clean;
    if (clean.length <= 6) return `(${clean.slice(0, 3)}) ${clean.slice(3)}`;
    return `(${clean.slice(0, 3)}) ${clean.slice(3, 6)}-${clean.slice(6)}`;
  }
  if (iso === 'in') {
    // 98765 43210 (10 digits)
    const clean = digits.slice(0, 10);
    if (clean.length <= 5) return clean;
    return `${clean.slice(0, 5)} ${clean.slice(5)}`;
  }
  if (iso === 'ae' || iso === 'sa') {
    // 50 123 4567 (9 digits)
    const clean = digits.slice(0, 9);
    if (clean.length <= 2) return clean;
    if (clean.length <= 5) return `${clean.slice(0, 2)} ${clean.slice(2)}`;
    return `${clean.slice(0, 2)} ${clean.slice(2, 5)} ${clean.slice(5)}`;
  }
  return digits.slice(0, 12);
}

@Component({
  selector: 'app-country-selector',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="country-picker">
      <button type="button" class="trigger-btn" (click)="toggleDropdown($event)">
        <img [src]="flagUrl(selected().iso)" [alt]="selected().name" class="flag-img" />
        <span class="code-text" *ngIf="mode === 'phone'">{{ selected().code }}</span>
        <span class="code-text" *ngIf="mode === 'id'">{{ selected().idCardName }}</span>
        <span class="arrow">▾</span>
      </button>

      <div class="dropdown-menu" *ngIf="isOpen()">
        <div
          class="dropdown-item"
          *ngFor="let c of countries"
          [class.active]="c.iso === selected().iso"
          (click)="selectCountry(c)"
        >
          <img [src]="flagUrl(c.iso)" [alt]="c.name" class="flag-img" />
          <span class="c-name">{{ c.name }}</span>
          <span class="c-code" *ngIf="mode === 'phone'">({{ c.code }})</span>
          <span class="c-code" *ngIf="mode === 'id'">({{ c.idCardName }})</span>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .country-picker {
      position: relative;
      display: inline-block;
    }
    .trigger-btn {
      display: flex;
      align-items: center;
      gap: 6px;
      height: 38px;
      padding: 0 10px;
      background: #f8fafc;
      border: 1.5px solid #cbd5e1;
      border-radius: 10px;
      font-size: 0.85rem;
      font-weight: 600;
      color: #0f172a;
      cursor: pointer;
      white-space: nowrap;
      transition: all 0.2s ease;
    }
    .trigger-btn:hover, .trigger-btn:focus {
      border-color: #087f8c;
      background: #ffffff;
      box-shadow: 0 0 0 3px rgba(8, 127, 140, 0.12);
    }
    .flag-img {
      width: 20px;
      height: 14px;
      object-fit: cover;
      border-radius: 2px;
      box-shadow: 0 1px 3px rgba(0,0,0,0.2);
    }
    .code-text {
      font-weight: 700;
    }
    .arrow {
      font-size: 0.75rem;
      color: #64748b;
    }
    .dropdown-menu {
      position: absolute;
      top: 100%;
      left: 0;
      z-index: 1000;
      margin-top: 4px;
      max-height: 220px;
      width: 230px;
      overflow-y: auto;
      background: #ffffff;
      border: 1.5px solid #e2e8f0;
      border-radius: 12px;
      box-shadow: 0 12px 30px rgba(15, 23, 42, 0.15);
      padding: 4px;
    }
    .dropdown-item {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 8px 10px;
      border-radius: 8px;
      font-size: 0.84rem;
      cursor: pointer;
      transition: background 0.15s ease;
    }
    .dropdown-item:hover {
      background: #f1f5f9;
    }
    .dropdown-item.active {
      background: #e6fffa;
      color: #087f8c;
      font-weight: 700;
    }
    .c-name {
      flex: 1;
      color: #1e293b;
    }
    .c-code {
      font-size: 0.78rem;
      color: #64748b;
    }
  `],
})
export class CountrySelectorComponent {
  @Input() mode: 'phone' | 'id' = 'phone';
  @Output() countrySelected = new EventEmitter<Country>();

  readonly countries = COUNTRIES;
  readonly selected = signal<Country>(COUNTRIES[0]);
  readonly isOpen = signal(false);

  constructor(private elRef: ElementRef) {}

  flagUrl(iso: string): string {
    return `https://flagcdn.com/w40/${iso}.png`;
  }

  toggleDropdown(event: MouseEvent): void {
    event.stopPropagation();
    this.isOpen.set(!this.isOpen());
  }

  selectCountry(country: Country): void {
    this.selected.set(country);
    this.isOpen.set(false);
    this.countrySelected.emit(country);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.elRef.nativeElement.contains(event.target)) {
      this.isOpen.set(false);
    }
  }
}
