import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SANodeComponent } from './sa-node.component';

describe('SANodeComponent', () => {
  let component: SANodeComponent;
  let fixture: ComponentFixture<SANodeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SANodeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SANodeComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
