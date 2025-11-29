import { TestBed } from '@angular/core/testing';

import { CommonStateManagerService } from './common-state-manager.service';
import { SADocument } from './model';

describe('CommonStateManagerService', () => {
  let service: CommonStateManagerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CommonStateManagerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

});