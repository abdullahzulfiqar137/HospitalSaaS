import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../../environments/environment';
import { LaboratoryService } from './laboratory.service';
describe('LaboratoryService', () => {
  let service: LaboratoryService;
  let http: HttpTestingController;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(LaboratoryService);
    http = TestBed.inject(HttpTestingController);
  });
  afterEach(() => http.verify());
  it('records a result on the backend result route', () => {
    const body = { itemId: 'item-id', result: '5.2', unit: 'mmol/L', referenceRange: '4-6' };
    service.recordResult('order-id', body).subscribe();
    const request = http.expectOne(`${environment.apiUrl}/laboratory/orders/order-id/results`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(body);
    request.flush({});
  });
  it('loads versioned result history', () => {
    service.resultHistory('order-id').subscribe();
    const request = http.expectOne(
      `${environment.apiUrl}/laboratory/orders/order-id/results/history`,
    );
    expect(request.request.method).toBe('GET');
    request.flush([]);
  });
});
