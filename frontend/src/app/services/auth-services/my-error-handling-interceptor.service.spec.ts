import { TestBed } from '@angular/core/testing';

import { MyErrorHandlingInterceptorService } from './my-error-handling-interceptor.service';

describe('MyErrorHandlingInterceptorService', () => {
  let service: MyErrorHandlingInterceptorService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MyErrorHandlingInterceptorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
