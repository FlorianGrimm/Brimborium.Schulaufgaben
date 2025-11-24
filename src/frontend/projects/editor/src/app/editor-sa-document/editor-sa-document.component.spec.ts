import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditorSADocumentComponent } from './editor-sa-document.component';

describe('EditorSADocumentComponent', () => {
  let component: EditorSADocumentComponent;
  let fixture: ComponentFixture<EditorSADocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditorSADocumentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditorSADocumentComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
