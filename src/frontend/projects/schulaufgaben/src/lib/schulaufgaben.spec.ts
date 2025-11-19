import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Schulaufgaben } from './schulaufgaben';

describe('Schulaufgaben', () => {
  let component: Schulaufgaben;
  let fixture: ComponentFixture<Schulaufgaben>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Schulaufgaben]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Schulaufgaben);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
