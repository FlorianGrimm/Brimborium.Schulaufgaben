import { TestBed } from '@angular/core/testing';

import { CommonCommandService } from './common-command.service';

describe('CommonCommandService', () => {
  let service: CommonCommandService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CommonCommandService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
