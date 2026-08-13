import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../../environments/environment';
import { AppointmentService } from './appointment.service';

describe('AppointmentService', () => {
  let service: AppointmentService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(AppointmentService);
    http = TestBed.inject(HttpTestingController);
  });
  afterEach(() => http.verify());

  it('maps appointment filters to backend query parameters', () => {
    service.search({ doctorId: 'doctor-id', status: 'Confirmed', pageNumber: 2 }).subscribe();
    const request = http.expectOne(
      (candidate) => candidate.url === `${environment.apiUrl}/appointments`,
    );
    expect(request.request.params.get('doctorId')).toBe('doctor-id');
    expect(request.request.params.get('status')).toBe('Confirmed');
    expect(request.request.params.get('pageNumber')).toBe('2');
    request.flush({
      items: [],
      pageNumber: 2,
      pageSize: 20,
      totalCount: 0,
      totalPages: 0,
      hasPreviousPage: true,
      hasNextPage: false,
    });
  });

  it('posts a typed appointment action', () => {
    const body = {
      action: 'Cancel' as const,
      reason: 'Patient request',
      appointmentDateTimeUtc: null,
      durationMinutes: null,
    };
    service.action('appointment-id', body).subscribe();
    const request = http.expectOne(`${environment.apiUrl}/appointments/appointment-id/actions`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(body);
    request.flush({});
  });
});
