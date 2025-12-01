import { Component, inject, input, OnInit, TemplateRef, ViewChild, ViewContainerRef } from '@angular/core';
import { Router } from '@angular/router';
import { BaseComponent, SADocument, SADocumentDescription, SANode, SAScalarUnit, SchulaufgabenEditorWebV1Service, SelectionService, CommonPageSizeComponent, createSubjectObservable, BoundObjectPath, BoundObjectPathValue, SADocumentRulerHorizontal, SADocumentRulerVertical, bindProperty, applyBoundPath, loadDocument } from 'schulaufgaben';
import { EditorSADocumentComponent } from '../editor-sa-document/editor-sa-document.component';
import { EditorRulerComponent } from '../editor-ruler/editor-ruler.component';
import { CommonDocumentManagerService, CommonStateManagerService } from 'schulaufgaben';
import { AsyncPipe } from '@angular/common';
import { BehaviorSubject, combineLatest, map } from 'rxjs';
import { apply } from '@angular/forms/signals';
import { MatIcon } from "@angular/material/icon";
import { TemplatePortal } from '@angular/cdk/portal';
import { is } from 'zod/locales';

type ViewState = {
  Document: BoundObjectPathValue<SADocument | null> | undefined;
  isDirty: boolean;
  RulerHorizontal: SAScalarUnit[];
  RulerVertical: SAScalarUnit[];
};
const initialViewState: ViewState = {
  Document: undefined,
  isDirty: false,
  RulerHorizontal: [],
  RulerVertical: [],
};


@Component({
  selector: 'app-editor-edit',
  imports: [
    AsyncPipe,
    EditorSADocumentComponent,
    EditorRulerComponent,
    CommonPageSizeComponent,
    MatIcon
],
  templateUrl: './editor-edit.component.html',
  styleUrl: './editor-edit.component.scss',
})
export class EditorEditComponent extends BaseComponent implements OnInit {
  readonly documentId = input.required<string>();
  
  private readonly _viewContainerRef = inject(ViewContainerRef);

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
      combineLatest({
        document:this.document$,
        isDirty: this.stateManagerService.documentManagerService.isDirty$
      }).subscribe({
        next: ({document,isDirty}) => {
          if (document == null || document.value == null) {
            this.viewState$.next(initialViewState);
          } else {
            const viewState = this.viewState$.getValue();
            const nextViewState: ViewState = {
              ...viewState,
              Document: document,
              isDirty: isDirty,
              RulerHorizontal: document.value.RulerHorizontal ?? [],
              RulerVertical: document.value.RulerVertical ?? [],
            };
            this.viewState$.next(nextViewState);
          }
        }
      })
    );

    this.load();
  }

  @ViewChild('contentPortalContent', { static: false }) contentPortalContent: TemplateRef<unknown> | undefined;
  templatePortal: TemplatePortal<any> | undefined;
  ngAfterViewInit(): void {
    if (undefined === this.contentPortalContent) {
      console.error('contentPortalContent is undefined');
    } else {
      const templatePortal = (this.templatePortal ??= new TemplatePortal(this.contentPortalContent, this._viewContainerRef));
      this.selectionService.setToolbarContent(templatePortal, this.subscriptions);
    }
  }

  load() {
    const id = this.documentId();
    if (id == null) { return; }
    if (this.stateManagerService.documentManagerService.document$.getValue()?.Id === id) {
      // skip
    } else {
      this.subscriptions.add(
        loadDocument(id, this.client).subscribe({
          next: (document) => {
            if (document == null) {
              return;
            } else {
              this.stateManagerService.documentManagerService.setDocumentState(document, 'load');
            }
          }
        }));
    }
  }

  handleVerticalChange(value: SAScalarUnit[]) {
    const documentBP = this.document$.getValue();
    // console.log("handleVerticalChange", documentBP?.value);

    if (documentBP?.value == null) { return; }
    const rulerBP = bindProperty(documentBP, "RulerVertical");
    const nextDocument = applyBoundPath(documentBP.value, rulerBP.opath, value);
    if (nextDocument == null) { return; }

    // console.log("nextDocument", nextDocument);
    this.stateManagerService.documentManagerService.pushDocumentState(nextDocument, "RulerVertical");
  }

  handleHorizontalChange(value: SAScalarUnit[]) {
    const documentBP = this.document$.getValue();
    // console.log("handleHorizontalChange", documentBP?.value);

    if (documentBP?.value == null) { return; }
    const rulerBP = bindProperty(documentBP, "RulerHorizontal");
    const nextDocument = applyBoundPath(documentBP.value, rulerBP.opath, value);
    if (nextDocument == null) { return; }

    // console.log("nextDocument", nextDocument);
    this.stateManagerService.documentManagerService.pushDocumentState(nextDocument, "RulerHorizontal");
  }

  save() {
    const document = this.document$.getValue()?.value;
    if (document == null) { return; }
    this.client.postAPIDocumentId(document.Id!, document).subscribe({
      next: (result) => {
        this.stateManagerService.documentManagerService.setDocumentState(result, 'save');
      }
    });
  }

}
