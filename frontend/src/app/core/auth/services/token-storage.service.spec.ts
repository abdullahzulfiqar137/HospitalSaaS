import { TestBed } from '@angular/core/testing';
import { LoginResponse } from '../models/auth.models';
import { TokenStorageService } from './token-storage.service';

describe('TokenStorageService', () => {
  let service: TokenStorageService;
  const tokens = {
    accessToken: 'access',
    refreshToken: 'refresh',
    expiresAtUtc: '2030-01-01T00:00:00Z',
  } as LoginResponse;

  beforeEach(() => {
    sessionStorage.clear();
    TestBed.configureTestingModule({});
    service = TestBed.inject(TokenStorageService);
  });

  afterEach(() => sessionStorage.clear());

  it('stores and retrieves a token response', () => {
    service.set(tokens);
    expect(service.get()).toEqual({
      accessToken: tokens.accessToken,
      expiresAtUtc: tokens.expiresAtUtc,
    });
    expect(sessionStorage.getItem('hospital.auth')).not.toContain('refresh');
  });

  it('clears stored authentication', () => {
    service.set(tokens);
    service.clear();
    expect(service.get()).toBeNull();
  });

  it('returns null for corrupted storage', () => {
    sessionStorage.setItem('hospital.auth', '{invalid');
    expect(service.get()).toBeNull();
  });
});
