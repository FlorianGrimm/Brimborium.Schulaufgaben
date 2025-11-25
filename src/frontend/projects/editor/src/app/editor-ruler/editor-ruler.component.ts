import { Component, inject, Input, output, OnInit, ElementRef } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { BaseComponent, CommonPageSizeService, convertionDocumentToPageInitialValue, createSubjectObservable, SAScalarUnit } from 'schulaufgaben';
import { AsyncPipe } from '@angular/common';

/**
 * A component to display 2 ruler horizontal and vertical.
 * Every 10% a ruler mark is displayed.
 * You can modify the list of horizontal and vertical user defined guide lines.
 * You can drag the guide lines.
 * You can add new guide lines.
 * You can delete guide lines.
 */
@Component({
  selector: 'app-editor-ruler',
  imports: [AsyncPipe],
  templateUrl: './editor-ruler.component.html',
  styleUrl: './editor-ruler.component.scss',
})
export class EditorRulerComponent extends BaseComponent implements OnInit {
  readonly commonPageSizeService = inject(CommonPageSizeService);
  private readonly hostElement = inject(ElementRef<HTMLElement>);

  readonly convertionDocumentToPage$ = createSubjectObservable({
    initialValue: convertionDocumentToPageInitialValue,
    observable: this.commonPageSizeService.convertionDocumentToPage$,
    subscription: this.subscriptions
  });

  readonly showGuides$ = new BehaviorSubject<boolean>(true);
  public get showGuides(): boolean {
    return this.showGuides$.getValue();
  }
  @Input()
  public set showGuides(value: boolean) {
    this.showGuides$.next(value);
  }

  readonly listHorizontal$ = new BehaviorSubject<SAScalarUnit[]>([]);
  public get listHorizontal(): SAScalarUnit[] {
    return this.listHorizontal$.getValue();
  }
  @Input()
  public set listHorizontal(value: SAScalarUnit[]) {
    this.listHorizontal$.next(value);
  }
  readonly onListHorizontalChanged = output<SAScalarUnit[]>();

  readonly listVertical$ = new BehaviorSubject<SAScalarUnit[]>([]);
  public get listVertical(): SAScalarUnit[] {
    return this.listVertical$.getValue();
  }
  @Input()
  public set listVertical(value: SAScalarUnit[]) {
    this.listVertical$.next(value);
  }
  readonly onListVerticalChanged = output<SAScalarUnit[]>();

  // Ruler marks at 10% intervals
  readonly rulerMarks = [0, 10, 20, 30, 40, 50, 60, 70, 80, 90];

  // Dragging state
  private draggedGuide: { type: 'horizontal' | 'vertical', index: number } | null = null;

  constructor() {
    super();
  }

  ngOnInit(): void {
    // No need for manual subscription - using createSubjectObservable
  }

  /**
   * Get the pixel position for a ruler mark based on its percentage value
   */
  getRulerMarkPosition(mark: number, type: 'horizontal' | 'vertical'): number {
    const conversion = this.convertionDocumentToPage$.getValue();
    if (type === 'horizontal') {
      // Horizontal ruler marks are positioned horizontally (left to right)
      return conversion.leftPadding + (mark / 100) * conversion.playgroundSize.width;
    } else {
      // Vertical ruler marks are positioned vertically (top to bottom)
      return conversion.topPadding + (mark / 100) * conversion.playgroundSize.height;
    }
  }

  /**
   * Get the pixel position for a guide line based on its percentage value
   */
  getGuidePosition(guide: SAScalarUnit, type: 'horizontal' | 'vertical'): number {
    // Assuming guide.Value is a percentage (0-100)
    const padding = this.commonPageSizeService.getPaddingForGuides(type);
    const value=this.commonPageSizeService.convertToPixelForGuides(guide, type);
    console.log("getGuidePosition", {guide, type, padding, value});
    return padding + value;;
  }

  /**
   * Handle clicking on the ruler to add a new guide line
   */
  onRulerClick(event: MouseEvent, type: 'horizontal' | 'vertical'): void {
    const rect = this.hostElement.nativeElement.getBoundingClientRect();
    const conversion = this.convertionDocumentToPage$.getValue();

    let percentage: number;

    if (type === 'horizontal') {
      // Click on horizontal ruler (top) - creates horizontal guide line
      const y = event.clientY - rect.top;
      const relativeY = y - conversion.topPadding;
      const playgroundHeight = conversion.playgroundSize.height;
      percentage = Math.max(0, Math.min(100, (relativeY / playgroundHeight) * 100));

      const newGuide: SAScalarUnit = {
        Value: percentage,
        Unit: 0, // Percent
      };
      console.log("onRulerClick", {y, relativeY, playgroundHeight, percentage, newGuide}); 

      const newList = [...this.listHorizontal, newGuide];
      this.listHorizontal$.next(newList);
      this.onListHorizontalChanged.emit(newList);
    } else {
      // Click on vertical ruler (left) - creates vertical guide line
      const x = event.clientX - rect.left;
      const relativeX = x - conversion.leftPadding;
      const playgroundWidth = conversion.playgroundSize.width;
      percentage = Math.max(0, Math.min(100, (relativeX / playgroundWidth) * 100));

      const newGuide: SAScalarUnit = {
        Value: percentage,
        Unit: 0, // Percent
      };
      console.log("onRulerClick", {x, relativeX, playgroundWidth, percentage, newGuide});

      const newList = [...this.listVertical, newGuide];
      this.listVertical$.next(newList);
      this.onListVerticalChanged.emit(newList);
    }
  }

  /**
   * Start dragging a guide line
   */
  onGuideMouseDown(event: MouseEvent, type: 'horizontal' | 'vertical', index: number): void {
    event.preventDefault();
    event.stopPropagation();

    this.draggedGuide = { type, index };

    // Add global mouse move and mouse up listeners
    document.addEventListener('mousemove', this.onDocumentMouseMove);
    document.addEventListener('mouseup', this.onDocumentMouseUp);
  }

  /**
   * Handle mouse move while dragging
   */
  private onDocumentMouseMove = (event: MouseEvent): void => {
    if (!this.draggedGuide) return;

    const rect = this.hostElement.nativeElement.getBoundingClientRect();
    const conversion = this.convertionDocumentToPage$.getValue();

    const { type, index } = this.draggedGuide;

    if (type === 'horizontal') {
      const y = event.clientY - rect.top;
      const relativeY = y - conversion.topPadding;
      const playgroundHeight = conversion.playgroundSize.height;
      const percentage = Math.max(0, Math.min(100, (relativeY / playgroundHeight) * 100));

      const newList = [...this.listHorizontal];
      newList[index] = { ...newList[index], Value: percentage };
      this.listHorizontal$.next(newList);
    } else {
      const x = event.clientX - rect.left;
      const relativeX = x - conversion.leftPadding;
      const playgroundWidth = conversion.playgroundSize.width;
      const percentage = Math.max(0, Math.min(100, (relativeX / playgroundWidth) * 100));

      const newList = [...this.listVertical];
      newList[index] = { ...newList[index], Value: percentage };
      this.listVertical$.next(newList);
    }
  };

  /**
   * Handle mouse up to finish dragging
   */
  private onDocumentMouseUp = (event: MouseEvent): void => {
    if (!this.draggedGuide) return;

    const { type } = this.draggedGuide;

    // Check if the guide was dragged outside the ruler area (delete it)
    const rect = this.hostElement.nativeElement.getBoundingClientRect();
    const isOutside = type === 'horizontal'
      ? (event.clientX < rect.left || event.clientX > rect.right)
      : (event.clientY < rect.top || event.clientY > rect.bottom);

    if (isOutside) {
      // Delete the guide
      if (type === 'horizontal') {
        const newList = this.listHorizontal.filter((_, i) => i !== this.draggedGuide!.index);
        this.listHorizontal$.next(newList);
        this.onListHorizontalChanged.emit(newList);
      } else {
        const newList = this.listVertical.filter((_, i) => i !== this.draggedGuide!.index);
        this.listVertical$.next(newList);
        this.onListVerticalChanged.emit(newList);
      }
    } else {
      // Emit the updated list
      if (type === 'horizontal') {
        this.onListHorizontalChanged.emit(this.listHorizontal);
      } else {
        this.onListVerticalChanged.emit(this.listVertical);
      }
    }

    this.draggedGuide = null;
    document.removeEventListener('mousemove', this.onDocumentMouseMove);
    document.removeEventListener('mouseup', this.onDocumentMouseUp);
  };

  /**
   * Clean up event listeners on destroy
   */
  override ngOnDestroy(): void {
    document.removeEventListener('mousemove', this.onDocumentMouseMove);
    document.removeEventListener('mouseup', this.onDocumentMouseUp);
    super.ngOnDestroy();
  }
}
