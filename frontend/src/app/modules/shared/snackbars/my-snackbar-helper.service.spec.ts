import { TestBed } from '@angular/core/testing';

import { MySnackbarHelperService } from './my-snackbar-helper.service';

describe('MySnackbarHelperService', () => {
  let service: MySnackbarHelperService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MySnackbarHelperService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
