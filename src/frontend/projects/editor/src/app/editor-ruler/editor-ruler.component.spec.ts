import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditorRulerComponent } from './editor-ruler.component';

describe('EditorRulerComponent', () => {
  let component: EditorRulerComponent;
  let fixture: ComponentFixture<EditorRulerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditorRulerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditorRulerComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
