import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LoginResponse } from '../models/auth.models';
import { AuthService } from '../services/auth.service';
import { authInterceptor } from './auth.interceptor';

describe('authInterceptor', () => {
  let client: HttpClient;
  let http: HttpTestingController;
  const tokens = signal({ accessToken: 'access-token', expiresAtUtc: '2099-01-01T00:00:00Z' });
  const auth = {
    tokens: tokens.asReadonly(),
    isAuthenticated: signal(true).asReadonly(),
    refresh: jasmine.createSpy('refresh'),
    clear: jasmine.createSpy('clear'),
  };

  beforeEach(() => {
    auth.refresh.calls.reset();
    auth.clear.calls.reset();
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
        { provide: AuthService, useValue: auth },
      ],
    });
    client = TestBed.inject(HttpClient);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('adds a bearer token only to the configured API', () => {
    client.get(`${environment.apiUrl}/patients`).subscribe();
    const apiRequest = http.expectOne(`${environment.apiUrl}/patients`);
    expect(apiRequest.request.headers.get('Authorization')).toBe('Bearer access-token');
    apiRequest.flush({});

    client.get('https://example.org/public').subscribe();
    const externalRequest = http.expectOne('https://example.org/public');
    expect(externalRequest.request.headers.has('Authorization')).toBeFalse();
    externalRequest.flush({});
  });

  it('refreshes and retries an API request after an unauthorized response', () => {
    const refreshed: LoginResponse = {
      accessToken: 'new-token',
      refreshToken: '',
      expiresAtUtc: '2099-01-01T00:00:00Z',
    };
    auth.refresh.and.returnValue(of(refreshed));

    client.get(`${environment.apiUrl}/patients`).subscribe();
    http
      .expectOne(`${environment.apiUrl}/patients`)
      .flush({}, { status: 401, statusText: 'Unauthorized' });

    const retry = http.expectOne(`${environment.apiUrl}/patients`);
    expect(retry.request.headers.get('Authorization')).toBe('Bearer new-token');
    expect(auth.refresh).toHaveBeenCalledTimes(1);
    retry.flush({});
  });
});
