import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClientSANodeComponent } from './client-sa-node.component';

describe('ClientSANodeComponent', () => {
  let component: ClientSANodeComponent;
  let fixture: ComponentFixture<ClientSANodeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClientSANodeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClientSANodeComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
