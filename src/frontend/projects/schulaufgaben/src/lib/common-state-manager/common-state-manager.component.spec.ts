import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommonStateManagerComponent } from './common-state-manager.component';

describe('CommonStateManagerComponent', () => {
  let component: CommonStateManagerComponent;
  let fixture: ComponentFixture<CommonStateManagerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommonStateManagerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CommonStateManagerComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
