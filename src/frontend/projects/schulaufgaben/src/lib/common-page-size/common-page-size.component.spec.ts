import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommonPageSizeComponent } from './common-page-size.component';

describe('CommonPageSizeComponent', () => {
  let component: CommonPageSizeComponent;
  let fixture: ComponentFixture<CommonPageSizeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommonPageSizeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CommonPageSizeComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
