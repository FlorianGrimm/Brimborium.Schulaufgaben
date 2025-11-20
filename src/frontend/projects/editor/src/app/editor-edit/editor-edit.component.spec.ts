import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditorEditComponent } from './editor-edit.component';

describe('EditorEditComponent', () => {
  let component: EditorEditComponent;
  let fixture: ComponentFixture<EditorEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditorEditComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditorEditComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
