import { TestBed } from '@angular/core/testing';
import { LocaleService } from './locale.service';

describe('LocaleService', () => {
  let service: LocaleService;

  beforeEach(() => {
    localStorage.removeItem('hospital.locale');
    TestBed.configureTestingModule({});
    service = TestBed.inject(LocaleService);
  });

  afterEach(() => {
    localStorage.removeItem('hospital.locale');
    document.documentElement.lang = 'en';
    document.documentElement.dir = 'ltr';
  });

  it('uses English and left-to-right layout by default', () => {
    expect(service.selected().code).toBe('en');
    expect(document.documentElement.dir).toBe('ltr');
  });

  it('persists Urdu and enables right-to-left layout', () => {
    service.set('ur');
    expect(service.selected().code).toBe('ur');
    expect(document.documentElement.lang).toBe('ur');
    expect(document.documentElement.dir).toBe('rtl');
    expect(localStorage.getItem('hospital.locale')).toBe('ur');
  });
});
