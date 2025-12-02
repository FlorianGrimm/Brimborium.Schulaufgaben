import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditorImageEditorComponent } from './editor-image-editor.component';

describe('EditorImageEditorComponent', () => {
  let component: EditorImageEditorComponent;
  let fixture: ComponentFixture<EditorImageEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditorImageEditorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditorImageEditorComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
