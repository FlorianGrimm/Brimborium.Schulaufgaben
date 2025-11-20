import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditorHomeComponent } from './editor-home.component';

describe('EditorHomeComponent', () => {
  let component: EditorHomeComponent;
  let fixture: ComponentFixture<EditorHomeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditorHomeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditorHomeComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
