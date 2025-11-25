import { Component, ElementRef, inject } from '@angular/core';
import { CommonPageSizeService } from '../common-page-size.service';
import { BaseComponent } from '../base-component';

@Component({
  selector: 'lib-common-page-size',
  imports: [],
  templateUrl: './common-page-size.component.html',
  styleUrl: './common-page-size.component.css'
})
export class CommonPageSizeComponent extends BaseComponent {
  readonly commonPageSizeService = inject(CommonPageSizeService);

  constructor(
    private host: ElementRef<HTMLDivElement>
  ) {
    super();
  }

  ngOnInit() {
    const observer = new ResizeObserver(entries => {
      const contentRect = entries[0].contentRect;
      this.commonPageSizeService.setPageSize({width: contentRect.width, height: contentRect.height});
    });
    observer.observe(this.host.nativeElement, { box: 'border-box' });
  }

}
