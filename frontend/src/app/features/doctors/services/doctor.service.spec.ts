import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../../environments/environment';
import { DoctorService } from './doctor.service';

describe('DoctorService', () => {
  let service: DoctorService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(DoctorService);
    http = TestBed.inject(HttpTestingController);
  });
  afterEach(() => http.verify());

  it('updates the exact doctor route', () => {
    const body = {
      firstName: 'A',
      lastName: 'Doctor',
      specialization: 'Cardiology',
      departmentId: 'department-id',
      licenseNumber: 'LIC-1',
      phone: null,
      email: null,
      consultationFee: 100,
    };
    service.update('doctor-id', body).subscribe();
    const request = http.expectOne(`${environment.apiUrl}/doctors/doctor-id`);
    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual(body);
    request.flush({});
  });

  it('activates a doctor through the action route', () => {
    service.activate('doctor-id').subscribe();
    const request = http.expectOne(`${environment.apiUrl}/doctors/doctor-id/activate`);
    expect(request.request.method).toBe('POST');
    request.flush(null);
  });
});
