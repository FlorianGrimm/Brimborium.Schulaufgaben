import { TestBed } from '@angular/core/testing';

import { CommonPageSizeService } from './common-page-size.service';

describe('CommonPageSizeService', () => {
  let service: CommonPageSizeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CommonPageSizeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
