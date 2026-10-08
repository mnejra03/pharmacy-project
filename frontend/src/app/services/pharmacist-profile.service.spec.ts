import { TestBed } from '@angular/core/testing';

import { PharmacistProfileService } from './pharmacist-profile.service';

describe('PharmacistProfileService', () => {
  let service: PharmacistProfileService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PharmacistProfileService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
