import { TestBed } from '@angular/core/testing';

import { ListWorkStateService } from './list-work-state.service';

describe('ListWorkStateService', () => {
  let service: ListWorkStateService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ListWorkStateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
