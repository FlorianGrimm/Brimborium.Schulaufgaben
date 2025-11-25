import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { SAScalarUnit } from './model';

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
  documentSize: SizePixel;
  pageSize: SizePixel;
  playgroundSize: SizePixel;
}
export const pageSizeInitialValue = { width: 1200, height: 800 };
export const documentSizeInitialValue = { width: 1200, height: 800 };
export const convertionDocumentToPageInitialValue = {
  factor: 1.0,
  leftPadding: 0,
  topPadding: 0,
  rightPadding: 0,
  bottomPadding: 0,
  documentSize: documentSizeInitialValue,
  pageSize: pageSizeInitialValue,
  playgroundSize: pageSizeInitialValue,
};


@Injectable({
  providedIn: 'root',
})
export class CommonPageSizeService {

  // the current page size  
  readonly pageSize$ = new BehaviorSubject<SizePixel>(pageSizeInitialValue);

  // the size of the document
  readonly documentSize$ = new BehaviorSubject<SizePixel>(documentSizeInitialValue);

  // the factor to fit the document into the page
  readonly convertionDocumentToPage$ = new BehaviorSubject<ConvertionDocumentToPage>(convertionDocumentToPageInitialValue);

  setPageSize(pageSize: SizePixel) {
    this.pageSize$.next(pageSize);
    this.calculateFactorSize();
  }

  setDocumentSize(documentSize: SizePixel) {
    this.documentSize$.next(documentSize);
    this.calculateFactorSize();
  }
  calculateFactorSize() {
    const pageSize = this.pageSize$.getValue();
    const documentSize = this.documentSize$.getValue();
    if (undefined === pageSize || undefined === documentSize) {
      this.convertionDocumentToPage$.next(convertionDocumentToPageInitialValue);
      return;
    }

    const factorWidth = pageSize.width / documentSize.width;
    const factorHeight = pageSize.height / documentSize.height;
    const factor = Math.min(factorWidth, factorHeight);
    const width = documentSize.width * factor;
    const height = documentSize.height * factor;
    const playgroundSize = { width: width, height: height, };
    const leftPadding = (pageSize.width - width) / 2;
    const topPadding = (pageSize.height - height) / 2;
    const rightPadding = pageSize.width - width - leftPadding;
    const bottomPadding = pageSize.height - height - topPadding;
    const nextValue: ConvertionDocumentToPage = ({
      factor: factor,
      leftPadding: leftPadding,
      topPadding: topPadding,
      rightPadding: rightPadding,
      bottomPadding: bottomPadding,
      documentSize: documentSize,
      pageSize: pageSize,
      playgroundSize: playgroundSize
    });
    // console.log("calculateFactorSize", { nextValue, documentSize, pageSize });
    this.convertionDocumentToPage$.next(nextValue);
  }

  getPaddingForGuides(type: 'horizontal' | 'vertical'): number {
    const conversion = this.convertionDocumentToPage$.getValue();
    if (type === 'horizontal') {
      return conversion.topPadding;
    } else {
      return conversion.leftPadding;
    }
  }


  convertToPixelForGuides(value: SAScalarUnit, type: 'horizontal' | 'vertical') {
    if (value.Unit === 0) { // Percent
      const conversion = this.convertionDocumentToPage$.getValue();
      if (type === 'horizontal') {
        return (value.Value / 100) * conversion.playgroundSize.height;
      } else {
        return (value.Value / 100) * conversion.playgroundSize.width;
      }
    }
    else if (value.Unit === 1) { // Pixel
      return value.Value;
    }
    return 0;
  }

  convertToPercent(value: SAScalarUnit, type: 'horizontal' | 'vertical') {
    if (value.Unit === 0) { // Percent
      return value.Value;
    } else if (value.Unit === 1) { // Pixel
      const conversion = this.convertionDocumentToPage$.getValue();
      if (type === 'horizontal') {
        return (value.Value / conversion.playgroundSize.width) * 100;
      } else {
        return (value.Value / conversion.playgroundSize.height) * 100;
      }
    }
    return 0;
  }

  convertToPixel(value: SAScalarUnit, type: 'horizontal' | 'vertical') {
    if (value.Unit === 0) { // Percent
      const conversion = this.convertionDocumentToPage$.getValue();
      if (type === 'horizontal') {
        return (value.Value / 100) * conversion.playgroundSize.width;
      } else {
        return (value.Value / 100) * conversion.playgroundSize.height;
      }
    }
    else if (value.Unit === 1) { // Pixel
      return value.Value;
    }
    return 0;
  }
}
