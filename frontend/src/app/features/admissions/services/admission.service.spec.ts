import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../../environments/environment';
import { Admission } from '../models/admission.model';
import { AdmissionService } from './admission.service';

describe('AdmissionService', () => {
  let service: AdmissionService;
  let http: HttpTestingController;
  const admission = { id: 'admission-id', admissionNumber: 'ADM-001' } as Admission;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(AdmissionService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('loads an admission by id', () => {
    service.get('admission-id').subscribe((value) => expect(value).toEqual(admission));
    const request = http.expectOne(`${environment.apiUrl}/admissions/admission-id`);
    expect(request.request.method).toBe('GET');
    request.flush(admission);
  });

  it('posts a typed transfer request', () => {
    const transfer = { wardId: 'ward', roomId: 'room', bedId: 'bed', reason: 'Clinical need' };
    service.transfer('admission-id', transfer).subscribe();
    const request = http.expectOne(`${environment.apiUrl}/admissions/admission-id/transfer`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(transfer);
    request.flush(admission);
  });

  it('posts discharge to the correct action route', () => {
    service.discharge('admission-id').subscribe();
    const request = http.expectOne(`${environment.apiUrl}/admissions/admission-id/discharge`);
    expect(request.request.method).toBe('POST');
    request.flush(null);
  });
});
