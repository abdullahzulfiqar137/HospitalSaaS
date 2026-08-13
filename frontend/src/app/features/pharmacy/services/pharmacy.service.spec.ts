import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../../environments/environment';
import { PharmacyService } from './pharmacy.service';
describe('PharmacyService', () => {
  let service: PharmacyService;
  let http: HttpTestingController;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(PharmacyService);
    http = TestBed.inject(HttpTestingController);
  });
  afterEach(() => http.verify());
  it('posts stock transactions to the medicine route', () => {
    const body = { type: 'Purchase' as const, quantity: 25, reference: 'PO-1' };
    service.stock('medicine-id', body).subscribe();
    const request = http.expectOne(`${environment.apiUrl}/pharmacy/medicines/medicine-id/stock`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(body);
    request.flush({});
  });
  it('loads current medicine stock', () => {
    service.balance('medicine-id').subscribe((value) => expect(value.quantity).toBe(25));
    const request = http.expectOne(`${environment.apiUrl}/pharmacy/medicines/medicine-id/stock`);
    request.flush({ quantity: 25 });
  });
});
