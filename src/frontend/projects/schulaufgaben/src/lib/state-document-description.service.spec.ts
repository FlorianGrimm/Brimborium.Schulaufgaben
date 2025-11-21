import { TestBed } from '@angular/core/testing';

import { StateDocumentDescriptionService } from './state-document-description.service';

describe('StateDocumentDescriptionService', () => {
  let service: StateDocumentDescriptionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StateDocumentDescriptionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
