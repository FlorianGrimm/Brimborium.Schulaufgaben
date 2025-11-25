import { Component, inject, Input, output } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { BaseComponent, CommonPageSizeService, SAScalarUnit } from 'schulaufgaben';

/**
 * A component to display 2 ruler horizontal and vertical.
 * Every 10% a ruler mark is displayed.
 * you can set the list of horizontal and vertical user defined guide lines.
 * The content is scaled to fit the page..
 */
@Component({
  selector: 'app-editor-ruler',
  imports: [],
  templateUrl: './editor-ruler.component.html',
  styleUrl: './editor-ruler.component.scss',
})
export class EditorRulerComponent extends BaseComponent {
  readonly commonPageSizeService = inject(CommonPageSizeService);

  readonly showGuides$ = new BehaviorSubject<boolean>(true);
  public get showGuides(): boolean {
    return this.showGuides$.getValue();
  }
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
  readonly onListHorizontalChanged=output();

  readonly listVertical$ = new BehaviorSubject<SAScalarUnit[]>([]);
  public get listVertical(): SAScalarUnit[] {
    return this.listVertical$.getValue();
  }  
  @Input()
  public set listVertical(value: SAScalarUnit[]) {
    this.listVertical$.next(value);
  }
  readonly onListVerticalChanged=output();

  constructor() {
    super();
  }
  
}
