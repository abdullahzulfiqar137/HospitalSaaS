import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../../environments/environment';
import { InventoryService } from './inventory.service';

describe('InventoryService', () => {
  let service: InventoryService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(InventoryService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('posts stock adjustments to the backend route', () => {
    const body = { quantity: 10, reason: 'Initial stock' };
    service.adjust('item-id', body).subscribe();
    const request = http.expectOne(`${environment.apiUrl}/inventory/item-id/adjustments`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(body);
    request.flush({});
  });

  it('loads stock quantity for an inventory item', () => {
    service.stock('item-id').subscribe((stock) => expect(stock.quantity).toBe(10));
    const request = http.expectOne(`${environment.apiUrl}/inventory/item-id/stock`);
    expect(request.request.method).toBe('GET');
    request.flush({ quantity: 10 });
  });
});
