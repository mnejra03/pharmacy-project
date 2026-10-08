import { TestBed } from '@angular/core/testing';

import { MyPageProgressbarService } from './my-page-progressbar.service';

describe('MyPageProgressbarService', () => {
  let service: MyPageProgressbarService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MyPageProgressbarService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
