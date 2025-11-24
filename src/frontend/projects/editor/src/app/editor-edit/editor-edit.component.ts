import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BaseComponent, SADocument, SADocumentDescription, SADocumentSADocumentDecoration, SANode, SchulaufgabenEditorWebV1Service, SelectionService } from 'schulaufgaben';

@Component({
  selector: 'app-editor-edit',
  imports: [],
  templateUrl: './editor-edit.component.html',
  styleUrl: './editor-edit.component.scss',
})
export class EditorEditComponent extends BaseComponent implements OnInit {
  readonly router = inject(Router);
  readonly client = inject(SchulaufgabenEditorWebV1Service);
  readonly selectionService = inject(SelectionService);

  public documentDescription: SADocumentDescription | undefined;
  public document: SADocument | undefined;
  public decoration: SANode | undefined;
  constructor() {
    super();
  }

  ngOnInit(): void {
    this.load();
  }

  load() {
    //this.client.getAPIDocumentDescription

    //interaction?: SADocumentSADocumentInteraction;
    this.decoration = {
      $type: "SANode",
      id: "00000000-0000-0000-0000-000000000001",
      name: "decoration",
      kind: "decoration",
      listItem: [],
      position: undefined,
      normal: undefined,
      flipped: undefined,
      selected: undefined
    };
    this.document = {
      $type: "Document",
      id?: "00000000-0000-0000-0000-000000000000",
      name?: "test",
      description?: "test",
      kindInteraction?: "",
      listMedia: [],
      decoration: this.decoration,
      interaction: undefined,
      width: { $type: "SAScalarUnit", value: 100, unit: 0, name: "width" },
      height: { $type: "SAScalarUnit", value: 100, unit: 0, name: "height" },
      rulerHorizontal: [],
      rulerVertical: []
    };
  }

}
