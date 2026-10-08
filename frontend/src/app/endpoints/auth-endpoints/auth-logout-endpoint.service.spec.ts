import { TestBed } from '@angular/core/testing';

import { AuthLogoutEndpointService } from './auth-logout-endpoint.service';

describe('AuthLogoutEndpointService', () => {
  let service: AuthLogoutEndpointService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthLogoutEndpointService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
