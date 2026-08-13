import { Injectable } from '@angular/core';
import { SessionTokens } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class TokenStorageService {
  private readonly key = 'hospital.auth';

  get(): SessionTokens | null {
    try {
      const value = sessionStorage.getItem(this.key);
      return value ? (JSON.parse(value) as SessionTokens) : null;
    } catch {
      return null;
    }
  }

  set(tokens: SessionTokens): void {
    sessionStorage.setItem(
      this.key,
      JSON.stringify({ accessToken: tokens.accessToken, expiresAtUtc: tokens.expiresAtUtc }),
    );
  }

  clear(): void {
    sessionStorage.removeItem(this.key);
  }
}
