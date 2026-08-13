import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../../environments/environment';
import { BillingService } from './billing.service';

describe('BillingService', () => {
  let service: BillingService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(BillingService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('uses backend invoice filters', () => {
    service.list({ patientId: 'patient-id', status: 'Unpaid', pageNumber: 2 }).subscribe();
    const request = http.expectOne(
      `${environment.apiUrl}/billing/invoices?patientId=patient-id&status=Unpaid&pageNumber=2`,
    );
    expect(request.request.method).toBe('GET');
    request.flush({ items: [] });
  });

  it('records a payment through the invoice route', () => {
    const body = { amount: 150, method: 'Card' };
    service.recordPayment('invoice-id', body).subscribe();
    const request = http.expectOne(`${environment.apiUrl}/billing/invoices/invoice-id/payments`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(body);
    request.flush({});
  });
});
