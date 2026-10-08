import { TestBed } from '@angular/core/testing';

import { AuthRegisterEndpointService } from './auth-register-endpoint.service';

describe('AuthRegisterEndpointService', () => {
  let service: AuthRegisterEndpointService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthRegisterEndpointService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
