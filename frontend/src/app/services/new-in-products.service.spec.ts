import { TestBed } from '@angular/core/testing';

import { NewInProductsService } from './new-in-products.service';

describe('NewInProductsService', () => {
  let service: NewInProductsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NewInProductsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
