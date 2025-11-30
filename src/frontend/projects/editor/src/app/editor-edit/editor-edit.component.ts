import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BaseComponent, SADocument, SADocumentDescription, SANode, SAScalarUnit, SchulaufgabenEditorWebV1Service, SelectionService, CommonPageSizeComponent, createSubjectObservable, BoundObjectPath, BoundObjectPathValue, SADocumentRulerHorizontal, SADocumentRulerVertical, bindProperty } from 'schulaufgaben';
import { EditorSADocumentComponent } from '../editor-sa-document/editor-sa-document.component';
import { EditorRulerComponent } from '../editor-ruler/editor-ruler.component';
import { CommonDocumentManagerService, CommonStateManagerService } from 'schulaufgaben';
import { AsyncPipe } from '@angular/common';
import { BehaviorSubject, map } from 'rxjs';

type ViewState = {
  Document: BoundObjectPathValue<SADocument | null>| undefined;
  RulerHorizontal: SAScalarUnit[];
  RulerVertical: SAScalarUnit[];
};
const initialViewState: ViewState = {
  Document: undefined,
  RulerHorizontal: [],
  RulerVertical: [],
};


@Component({
  selector: 'app-editor-edit',
  imports: [
    AsyncPipe,
    EditorSADocumentComponent,
    EditorRulerComponent,
    CommonPageSizeComponent],
  templateUrl: './editor-edit.component.html',
  styleUrl: './editor-edit.component.scss',
})
export class EditorEditComponent extends BaseComponent implements OnInit {
  readonly viewState$ = new BehaviorSubject<ViewState>(initialViewState);
  readonly router = inject(Router);
  readonly client = inject(SchulaufgabenEditorWebV1Service);
  readonly selectionService = inject(SelectionService);
  readonly stateManagerService = inject(CommonStateManagerService);

  public document$ = createSubjectObservable(
    {
      initialValue: null,
      observable: this.stateManagerService.document$,
      subscription: this.subscriptions
    }
  );

  public documentDescription: SADocumentDescription | undefined;
  //public document: SADocument | undefined;
  public decoration: SANode | undefined;
  constructor() {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.add(
      this.document$.subscribe({
        next: (document) => {
          if (document == null || document.value == null) {
            this.viewState$.next(initialViewState);
          } else {
            const viewState = this.viewState$.getValue();
            const nextViewState:ViewState = {
              ...viewState,
              Document: document,
              RulerHorizontal: document.value.RulerHorizontal??[],
              RulerVertical: document.value.RulerVertical??[],
            };
            this.viewState$.next(nextViewState);
          }
        }
      })
    );
    this.load();
  }

  load() {
    //this.client.getAPIDocumentDescription

    //interaction?: SADocumentSADocumentInteraction;
    const decoration: SANode = {
      Id: "00000000-0000-0000-0000-000000000001",
      Name: "decoration",
      Kind: "decoration",
      ListItem: [],
      Position: undefined,
      Normal: undefined,
      Flipped: undefined,
      Selected: undefined
    };
    const interaction: SANode = {
      Id: "00000000-0000-0000-0000-000000000002",
      Name: "decoration",
      Kind: "decoration",
      ListItem: [],
      Position: undefined,
      Normal: undefined,
      Flipped: undefined,
      Selected: undefined
    };
    const document: SADocument = {
      Id: "00000000-0000-0000-0000-000000000000",
      Name: "test",
      Description: "test",
      KindInteraction: "",
      ListMedia: [],
      Decoration: decoration,
      Interaction: interaction,
      Width: { Value: 1200, Unit: 1, Name: "PageWidth" },
      Height: { Value: 800, Unit: 1, Name: "PageHeight" },
      RulerHorizontal: [{ Value: 1, Unit: 0, Name: "1%" }, { Value: 50, Unit: 0, Name: "50%" }, { Value: 99, Unit: 0, Name: "99%" }],
      RulerVertical: []//[{Value: 33, Unit: 0, Name: "33%"},{Value: 66, Unit: 0, Name: "66%"}]
    };
    this.stateManagerService.documentManagerService.document$.next(document);
  }

  handleVerticalChange(value: SAScalarUnit[]) {
    // console.log("handleVerticalChange", value);
    const document = this.document$.getValue();
    if (null == document) { return; }
    const nextDocument = { ...document, RulerVertical: value };
    this.stateManagerService.documentManagerService.pushDocumentState(nextDocument, "RulerHorizontal");
  }

  handleHorizontalChange(value: SAScalarUnit[]) {
    // console.log("handleHorizontalChange", value);
    const document = this.document$.getValue();
    if (null == document) { return; }
    const nextDocument = { ...document, RulerHorizontal: value };
    this.stateManagerService.documentManagerService.pushDocumentState(nextDocument, "RulerHorizontal");
  }

}
