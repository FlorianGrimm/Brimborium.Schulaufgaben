import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditorTransformationEditorComponent } from './editor-transformation-editor.component';

describe('EditorTransformationEditorComponent', () => {
  let component: EditorTransformationEditorComponent;
  let fixture: ComponentFixture<EditorTransformationEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditorTransformationEditorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditorTransformationEditorComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
