export interface LoginRequest {
  email: string;
  password: string;
}
export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAtUtc: string;
}
export type SessionTokens = Pick<LoginResponse, 'accessToken' | 'expiresAtUtc'>;
export interface CurrentUser {
  userId: string;
  tenantId: string;
  email: string;
  permissions: string[];
  roles: string[];
}
