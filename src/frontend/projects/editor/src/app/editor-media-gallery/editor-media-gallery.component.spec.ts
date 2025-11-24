import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditorMediaGalleryComponent } from './editor-media-gallery.component';

describe('EditorMediaGalleryComponent', () => {
  let component: EditorMediaGalleryComponent;
  let fixture: ComponentFixture<EditorMediaGalleryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditorMediaGalleryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditorMediaGalleryComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
