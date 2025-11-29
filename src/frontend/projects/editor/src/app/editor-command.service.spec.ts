import { TestBed } from '@angular/core/testing';

import { EditorCommandService } from './editor-command.service';

describe('EditorCommandService', () => {
  let service: EditorCommandService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EditorCommandService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
