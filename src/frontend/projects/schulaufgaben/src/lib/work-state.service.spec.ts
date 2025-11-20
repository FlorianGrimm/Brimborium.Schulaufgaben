import { TestBed } from '@angular/core/testing';

import { WorkStateService } from './work-state.service';

describe('WorkStateService', () => {
  let service: WorkStateService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(WorkStateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
