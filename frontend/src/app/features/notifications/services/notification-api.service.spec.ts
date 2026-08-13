import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../../environments/environment';
import { NotificationApiService } from './notification-api.service';

describe('NotificationApiService', () => {
  let service: NotificationApiService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(NotificationApiService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('uses notification filters supported by the backend', () => {
    service.list({ userId: 'user-id', status: 'Pending', pageSize: 10 }).subscribe();
    const request = http.expectOne(
      `${environment.apiUrl}/notifications?userId=user-id&status=Pending&pageSize=10`,
    );
    expect(request.request.method).toBe('GET');
    request.flush({ items: [] });
  });

  it('sends the complete notification request', () => {
    const body = { userId: 'user-id', subject: 'Result', body: 'Ready', channel: 'Email' };
    service.create(body).subscribe();
    const request = http.expectOne(`${environment.apiUrl}/notifications`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(body);
    request.flush({});
  });
});
