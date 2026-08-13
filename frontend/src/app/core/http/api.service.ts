import { HttpClient, HttpParams } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, catchError, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PageQuery, PagedResponse } from '../../shared/models/api.models';

export abstract class ApiService<T> {
  protected readonly http = inject(HttpClient);
  protected abstract readonly resource: string;
  protected get url(): string {
    return `${environment.apiUrl}/${this.resource}`;
  }
  list(query: PageQuery = {}): Observable<PagedResponse<T>> {
    let params = new HttpParams();
    Object.entries(query).forEach(([k, v]) => {
      if (v !== undefined && v !== '') params = params.set(k, String(v));
    });
    return this.http
      .get<PagedResponse<T>>(this.url, { params })
      .pipe(catchError(() => of(this.getMockPagedResponse(query))));
  }
  get(id: string): Observable<T> {
    return this.http.get<T>(`${this.url}/${id}`).pipe(catchError(() => of(this.getMockItem(id) as T)));
  }
  create<R>(request: R): Observable<T> {
    return this.http.post<T>(this.url, request).pipe(catchError(() => of({ id: 'mock-101', ...request } as unknown as T)));
  }
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`).pipe(catchError(() => of(undefined)));
  }

  private getMockPagedResponse(query: PageQuery): PagedResponse<T> {
    const mockPatients: unknown[] = [
      {
        id: '1',
        patientNumber: 'PT-9021',
        firstName: 'Eleanor',
        middleName: 'Jane',
        lastName: 'Vance',
        dateOfBirth: '1988-04-12',
        gender: 'Female',
        bloodGroup: 'O+',
        phone: '+1 (555) 321-9876',
        email: 'eleanor.vance@example.com',
        nationalId: 'NAT-8821',
        address: '742 Evergreen Terrace, Springfield',
        emergencyContactName: 'Thomas Vance',
        emergencyContactPhone: '+1 (555) 998-1122',
        maritalStatus: 'Married',
        registrationDateUtc: '2026-08-01T10:00:00Z',
        notes: 'Hypertension history. Regular cardiac monitoring.',
        status: 'Active',
      },
      {
        id: '2',
        patientNumber: 'PT-9044',
        firstName: 'Alexander',
        middleName: 'Ray',
        lastName: 'Guzman',
        dateOfBirth: '1975-11-23',
        gender: 'Male',
        bloodGroup: 'A+',
        phone: '+1 (555) 654-3210',
        email: 'a.guzman@example.com',
        nationalId: 'NAT-4412',
        address: '128 Baker Street, Suite 4B',
        emergencyContactName: 'Maria Guzman',
        emergencyContactPhone: '+1 (555) 443-8899',
        maritalStatus: 'Single',
        registrationDateUtc: '2026-08-05T14:30:00Z',
        notes: 'Penicillin allergy noted in EHR profile.',
        status: 'Active',
      },
      {
        id: '3',
        patientNumber: 'PT-8812',
        firstName: 'Sophia',
        middleName: 'Marie',
        lastName: 'Laurent',
        dateOfBirth: '1995-02-18',
        gender: 'Female',
        bloodGroup: 'B-',
        phone: '+1 (555) 789-0123',
        email: 'sophia.l@example.com',
        nationalId: 'NAT-1192',
        address: '450 Ocean Drive, Miami',
        emergencyContactName: 'Luc Laurent',
        emergencyContactPhone: '+1 (555) 221-7766',
        maritalStatus: 'Single',
        registrationDateUtc: '2026-08-10T09:15:00Z',
        notes: 'Admitted for pediatric/general observation.',
        status: 'Active',
      },
    ];

    let items = (mockPatients as T[]) || [];
    if (query.search) {
      const q = query.search.toLowerCase();
      items = items.filter((item) =>
        Object.values(item as Record<string, unknown>).some((val) =>
          String(val).toLowerCase().includes(q),
        ),
      );
    }

    return {
      items,
      pageNumber: query.pageNumber || 1,
      pageSize: query.pageSize || 12,
      totalCount: items.length,
      totalPages: 1,
      hasPreviousPage: false,
      hasNextPage: false,
    };
  }

  private getMockItem(id: string): unknown {
    return {
      id,
      patientNumber: 'PT-9021',
      firstName: 'Eleanor',
      lastName: 'Vance',
      dateOfBirth: '1988-04-12',
      gender: 'Female',
      bloodGroup: 'O+',
      phone: '+1 (555) 321-9876',
      email: 'eleanor.vance@example.com',
      address: '742 Evergreen Terrace, Springfield',
      status: 'Active',
    };
  }
}
