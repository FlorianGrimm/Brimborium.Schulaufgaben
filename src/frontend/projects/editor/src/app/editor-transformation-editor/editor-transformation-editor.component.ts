import { Component, Input, output } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { SAScalarUnit, SATransform, SAUnit, BaseComponent } from 'schulaufgaben';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { AsyncPipe } from '@angular/common';

/** Available transform functions */
export const TRANSFORM_FUNCTIONS = [
  { value: 'translate', label: 'Translate', argCount: 2, defaultUnit: 1 as SAUnit }, // px
  { value: 'scale', label: 'Scale', argCount: 2, defaultUnit: 3 as SAUnit }, // no unit
  { value: 'rotate', label: 'Rotate', argCount: 1, defaultUnit: 2 as SAUnit }, // deg
  { value: 'skew', label: 'Skew', argCount: 2, defaultUnit: 2 as SAUnit }, // deg
  { value: 'translateX', label: 'Translate X', argCount: 1, defaultUnit: 1 as SAUnit },
  { value: 'translateY', label: 'Translate Y', argCount: 1, defaultUnit: 1 as SAUnit },
  { value: 'scaleX', label: 'Scale X', argCount: 1, defaultUnit: 3 as SAUnit },
  { value: 'scaleY', label: 'Scale Y', argCount: 1, defaultUnit: 3 as SAUnit },
  { value: 'skewX', label: 'Skew X', argCount: 1, defaultUnit: 2 as SAUnit },
  { value: 'skewY', label: 'Skew Y', argCount: 1, defaultUnit: 2 as SAUnit },
];

@Component({
  selector: 'app-editor-transformation-editor',
  imports: [
    FormsModule,
    DragDropModule,
    MatSelectModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './editor-transformation-editor.component.html',
  styleUrl: './editor-transformation-editor.component.scss',
})
export class EditorTransformationEditorComponent extends BaseComponent {
  public listTransform$ = new BehaviorSubject<SATransform[]>([]);
  public listTransform: SATransform[] = [];
  public readonly onListTransformChanged = output<SATransform[]>();
  public readonly transformFunctions = TRANSFORM_FUNCTIONS;

  @Input()
  set inputListTransform(value: SATransform[] | undefined | null) {
    this.listTransform$.next(value ?? []);
  }

  constructor() {
    super();
    this.subscriptions.add(
      this.listTransform$.subscribe((value) => {
        this.listTransform = value;
      })
    );
  }

  /** Handle drag and drop reordering */
  onDrop(event: CdkDragDrop<SATransform[]>): void {
    const newList = [...this.listTransform];
    moveItemInArray(newList, event.previousIndex, event.currentIndex);
    this.listTransform$.next(newList);
    this.onListTransformChanged.emit(newList);
  }

  /** Add a new transform */
  addTransform(): void {
    const newTransform: SATransform = {
      Id: crypto.randomUUID(),
      TransformFunction: 'translate',
      ListArgument: [
        { Value: 0, Unit: 1 },
        { Value: 0, Unit: 1 },
      ],
    };
    const newList = [...this.listTransform, newTransform];
    this.listTransform$.next(newList);
    this.onListTransformChanged.emit(newList);
  }

  /** Remove a transform by index */
  removeTransform(index: number): void {
    const newList = this.listTransform.filter((_, i) => i !== index);
    this.listTransform$.next(newList);
    this.onListTransformChanged.emit(newList);
  }

  /** Update transform function and adjust arguments */
  onTransformFunctionChange(index: number, functionName: string): void {
    const newList = [...this.listTransform];
    const transform = { ...newList[index] };
    const funcDef = this.transformFunctions.find(f => f.value === functionName);

    transform.TransformFunction = functionName;

    // Adjust arguments to match new function
    if (funcDef) {
      const currentArgs = transform.ListArgument ?? [];
      const newArgs: SAScalarUnit[] = [];
      for (let i = 0; i < funcDef.argCount; i++) {
        if (currentArgs[i]) {
          newArgs.push({ ...currentArgs[i], Unit: funcDef.defaultUnit });
        } else {
          newArgs.push({ Value: 0, Unit: funcDef.defaultUnit });
        }
      }
      transform.ListArgument = newArgs;
    }

    newList[index] = transform;
    this.listTransform$.next(newList);
    this.onListTransformChanged.emit(newList);
  }

  /** Update argument value */
  onArgumentValueChange(transformIndex: number, argIndex: number, value: number): void {
    const newList = [...this.listTransform];
    const transform = { ...newList[transformIndex] };
    const args = [...(transform.ListArgument ?? [])];

    if (args[argIndex]) {
      args[argIndex] = { ...args[argIndex], Value: value };
    }

    transform.ListArgument = args;
    newList[transformIndex] = transform;
    this.listTransform$.next(newList);
    this.onListTransformChanged.emit(newList);
  }

  /** Get argument label based on transform function and argument index */
  getArgumentLabel(functionName: string | null | undefined, argIndex: number): string {
    if (!functionName) return `Arg ${argIndex + 1}`;

    switch (functionName) {
      case 'translate':
        return argIndex === 0 ? 'X' : 'Y';
      case 'scale':
        return argIndex === 0 ? 'X' : 'Y';
      case 'skew':
        return argIndex === 0 ? 'X' : 'Y';
      case 'rotate':
        return 'Angle';
      default:
        return 'Value';
    }
  }

  /** Get unit suffix for display */
  getUnitSuffix(unit: SAUnit): string {
    switch (unit) {
      case 0: return '%';
      case 1: return 'px';
      case 2: return 'deg';
      case 3: return '';
      default: return '';
    }
  }
}

export function convertListTransformToCss(listTransform:SATransform[]): string {
  if (listTransform.length === 0) {
    return "";
  }
  for (const transform of listTransform) {
    if (transform.TransformFunction) {
      if (transform.TransformFunction === "translate") {
        return "translate(" + transform.ListArgument?.map(arg => arg.Value + "px").join(",") + ")";
      } else if (transform.TransformFunction === "scale") {
        return "scale(" + transform.ListArgument?.map(arg => arg.Value).join(",") + ")";
      } else if (transform.TransformFunction === "rotate") {
        return "rotate(" + transform.ListArgument?.map(arg => arg.Value + "deg").join(",") + ")";
      } else if (transform.TransformFunction === "skew") {
        return "skew(" + transform.ListArgument?.map(arg => arg.Value + "deg").join(",") + ")";
      } else {
        return "";
      }
    }
  }
  return "";
}
export function convertTransformToCss(transform:SATransform): string {
     if (transform.TransformFunction) {
      if (transform.TransformFunction === "translate") {
        return "translate(" + transform.ListArgument?.map(arg => convertScalarUnitToString(arg , "px")).join(",") + ")";
      } else if (transform.TransformFunction === "scale") {
        return "scale(" + transform.ListArgument?.map(arg => convertScalarUnitToString(arg , "")).join(",") + ")";
      } else if (transform.TransformFunction === "rotate") {
        return "rotate(" + transform.ListArgument?.map(arg => convertScalarUnitToString(arg , "deg")).join(",") + ")";
      } else if (transform.TransformFunction === "skew") {
        return "skew(" + transform.ListArgument?.map(arg => convertScalarUnitToString(arg, "deg")).join(",") + ")";
      } else  {
        return transform.TransformFunction + "(" + transform.ListArgument?.map(arg => convertScalarUnitToString(arg, "")).join(",") + ")";
      }
    }
    return "";
}
export function convertScalarUnitToString(scalarUnit:SAScalarUnit, unit: string): string {
  if (scalarUnit.Unit === 0) {
    return scalarUnit.Value + "%";
  } else if (scalarUnit.Unit === 1) {
    return scalarUnit.Value + "px";
  } else if (scalarUnit.Unit === 2) {
    return scalarUnit.Value + "deg";
  } else if (scalarUnit.Unit === 3) {
    return scalarUnit.Value.toString();
  }
  // bad fallback
  return scalarUnit.Value.toString()+unit;
}