import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  HostListener,
  ViewChild,
  computed,
  inject,
  output,
  signal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/services/auth.service';
import { LocaleService } from '../../core/config/locale.service';
import { ThemeService } from '../../core/config/theme.service';

interface SearchItem {
  id: string;
  category: 'Patients' | 'Doctors' | 'Pharmacy' | 'Laboratory' | 'Billing' | 'Navigation';
  title: string;
  subtitle: string;
  icon: string;
  link: string;
}

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, FormsModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NavbarComponent {
  readonly auth = inject(AuthService);
  readonly theme = inject(ThemeService);
  readonly locale = inject(LocaleService);
  readonly menu = output<void>();

  readonly searchDropdownOpen = signal(false);
  readonly profileMenuOpen = signal(false);
  readonly searchQuery = signal('');

  @ViewChild('headerSearchInput') headerSearchInputEl?: ElementRef<HTMLInputElement>;

  readonly allSearchItems: SearchItem[] = [
    {
      id: 'p1',
      category: 'Patients',
      title: 'Eleanor Vance',
      subtitle: 'MRN: #PAT-8849 • Blood: O+ • Inpatient Ward A',
      icon: '👤',
      link: '/patients',
    },
    {
      id: 'p2',
      category: 'Patients',
      title: 'Marcus Vance',
      subtitle: 'MRN: #PAT-7712 • Blood: A+ • Outpatient Clinic',
      icon: '👤',
      link: '/patients',
    },
    {
      id: 'd1',
      category: 'Doctors',
      title: 'Dr. Sarah Jenkins',
      subtitle: 'Cardiology Specialist • MD, FACC • Dept #102',
      icon: '🩺',
      link: '/doctors',
    },
    {
      id: 'd2',
      category: 'Doctors',
      title: 'Dr. Alex Morgan',
      subtitle: 'Chief Medical Director • General Surgery',
      icon: '🩺',
      link: '/doctors',
    },
    {
      id: 'ph1',
      category: 'Pharmacy',
      title: 'Amoxicillin 500mg Capsules',
      subtitle: 'Stock: 450 units • Reorder Threshold: 100',
      icon: '💊',
      link: '/pharmacy',
    },
    {
      id: 'ph2',
      category: 'Pharmacy',
      title: 'Paracetamol Extra 500mg',
      subtitle: 'Stock: 1,200 units • Central Pharmacy Ward',
      icon: '💊',
      link: '/pharmacy',
    },
    {
      id: 'l1',
      category: 'Laboratory',
      title: 'Complete Blood Count (CBC)',
      subtitle: 'Pathology Test Catalog • Turnaround: 2 hrs',
      icon: '🧪',
      link: '/laboratory',
    },
    {
      id: 'b1',
      category: 'Billing',
      title: 'Invoice #INV-2026-004',
      subtitle: 'St. Jude Health • Amount: $1,450.00 • Status: Pending',
      icon: '📄',
      link: '/billing',
    },
    {
      id: 'n1',
      category: 'Navigation',
      title: 'Hospital Staff Directory',
      subtitle: 'Manage doctors, nurses, and clinical roster',
      icon: '⚙️',
      link: '/staff',
    },
    {
      id: 'n2',
      category: 'Navigation',
      title: 'Roles & Permissions Matrix',
      subtitle: 'Configure RBAC role scopes and access controls',
      icon: '🛡️',
      link: '/roles',
    },
  ];

  readonly filteredSearchResults = computed(() => {
    const q = this.searchQuery().trim().toLowerCase();
    if (!q) return this.allSearchItems;
    return this.allSearchItems.filter(
      (item) =>
        item.title.toLowerCase().includes(q) ||
        item.subtitle.toLowerCase().includes(q) ||
        item.category.toLowerCase().includes(q),
    );
  });

  @HostListener('window:keydown', ['$event'])
  handleKeyboardShortcut(event: KeyboardEvent): void {
    if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'k') {
      event.preventDefault();
      this.headerSearchInputEl?.nativeElement.focus();
      this.searchDropdownOpen.set(true);
    } else if (event.key === 'Escape' && this.searchDropdownOpen()) {
      this.closeSearchDropdown();
    }
  }

  openSearchDropdown(): void {
    this.searchDropdownOpen.set(true);
  }

  closeSearchDropdown(): void {
    this.searchDropdownOpen.set(false);
  }

  toggleProfileMenu(): void {
    this.profileMenuOpen.update((v) => !v);
  }

  closeProfileMenu(): void {
    this.profileMenuOpen.set(false);
  }
}
