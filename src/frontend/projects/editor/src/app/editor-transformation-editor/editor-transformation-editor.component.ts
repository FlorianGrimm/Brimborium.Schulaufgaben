import { Component } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { SAScalarUnit, SATransform } from 'schulaufgaben';
@Component({
  selector: 'app-editor-transformation-editor',
  imports: [],
  templateUrl: './editor-transformation-editor.component.html',
  styleUrl: './editor-transformation-editor.component.scss',
})
export class EditorTransformationEditorComponent {
  public listTransform$ = new BehaviorSubject<SATransform[]>([]);
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