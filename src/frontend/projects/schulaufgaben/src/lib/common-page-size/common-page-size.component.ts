import { AfterViewInit, Component, ElementRef, inject, OnInit } from '@angular/core';
import { CommonPageSizeService, convertionDocumentToPageInitialValue } from '../common-page-size.service';
import { BaseComponent } from '../base-component';
import { createSubjectObservable } from '../createSubjectObservable';
import { AsyncPipe, JsonPipe, NgStyle } from '@angular/common';

@Component({
  selector: 'lib-common-page-size',
  imports: [JsonPipe, AsyncPipe, NgStyle],
  templateUrl: './common-page-size.component.html',
  styleUrl: './common-page-size.component.css'
})
export class CommonPageSizeComponent extends BaseComponent implements OnInit {
  readonly commonPageSizeService = inject(CommonPageSizeService);

  readonly convertionDocumentToPage$ = createSubjectObservable({
    initialValue: convertionDocumentToPageInitialValue,
    observable: this.commonPageSizeService.convertionDocumentToPage$,
    subscription: this.subscriptions
  });

  constructor(
    private host: ElementRef<HTMLDivElement>
  ) {
    super();
  }

  ngOnInit() {
    const observer = new ResizeObserver(entries => {
      const contentRect = entries[0].contentRect;
      if (contentRect.width == 0 || contentRect.height == 0) {
        return;
      } else {
        this.commonPageSizeService.setPageSize({ width: contentRect.width, height: contentRect.height });
      }
    });
    observer.observe(this.host.nativeElement, { box: 'content-box' });
    this.subscriptions.add(() => { observer.disconnect(); });

    const contentRect = this.host.nativeElement.getBoundingClientRect();
    this.commonPageSizeService.setPageSize({ width: contentRect.width, height: contentRect.height });
  }
}
