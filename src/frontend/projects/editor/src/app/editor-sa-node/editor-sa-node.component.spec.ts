import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditorSANodeComponent } from './editor-sa-node.component';

describe('EditorSANodeComponent', () => {
  let component: EditorSANodeComponent;
  let fixture: ComponentFixture<EditorSANodeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditorSANodeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditorSANodeComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
