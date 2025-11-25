import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface SizePixel {
  width: number;
  height: number;
}
export interface ConvertionDocumentToPage {
  factor: number;
  leftPadding: number;
  topPadding: number;
  rightPadding: number;
  bottomPadding: number;
}

@Injectable({
  providedIn: 'root',
})
export class CommonPageSizeService {
  // the current page size  
  readonly pageSize$ = new BehaviorSubject<SizePixel | undefined>(undefined);

  // the size of the document
  readonly documentSize$ = new BehaviorSubject<SizePixel | undefined>(undefined);

  // the factor to fit the document into the page
  readonly convertionDocumentToPage$ = new BehaviorSubject<ConvertionDocumentToPage>({
    factor: 1.0,
    leftPadding: 0,
    topPadding: 0,
    rightPadding: 0,
    bottomPadding: 0,
  });

  setPageSize(pageSize:SizePixel) {
    this.pageSize$.next(pageSize);
    this.calculateFactorSize();
  }

  setDocumentSize(documentSize:SizePixel) {
    this.documentSize$.next(documentSize);
    this.calculateFactorSize();
  }
  calculateFactorSize() {
    const pageSize = this.pageSize$.getValue();
    const documentSize = this.documentSize$.getValue();
    if (undefined === pageSize || undefined === documentSize) {
      this.convertionDocumentToPage$.next({
        factor: 1.0,
        leftPadding: 0,
        topPadding: 0,
        rightPadding: 0,
        bottomPadding: 0,
      });
      return;
    }

    const factorWidth = pageSize.width / documentSize.width;
    const factorHeight = pageSize.height / documentSize.height;
    const factor = Math.min(factorWidth, factorHeight);
    const width = documentSize.width * factor;
    const height = documentSize.height * factor;
    const leftPadding = (pageSize.width - width) / 2;
    const topPadding = (pageSize.height - height) / 2;
    const rightPadding = pageSize.width - width - leftPadding;
    const bottomPadding = pageSize.height - height - topPadding;
    this.convertionDocumentToPage$.next({
      factor: factor,
      leftPadding: leftPadding,
      topPadding: topPadding,
      rightPadding: rightPadding,
      bottomPadding: bottomPadding,
    });
  }  
}
