import { TestBed } from '@angular/core/testing';

import { AuthLoginEndpointService } from './auth-login-endpoint.service';

describe('AuthLoginEndpointService', () => {
  let service: AuthLoginEndpointService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthLoginEndpointService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
