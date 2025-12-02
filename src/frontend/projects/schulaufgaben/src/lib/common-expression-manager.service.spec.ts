import { TestBed } from '@angular/core/testing';

import { CommonExpressionManagerService } from './common-expression-manager.service';

describe('CommonExpressionManagerService', () => {
  let service: CommonExpressionManagerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CommonExpressionManagerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
