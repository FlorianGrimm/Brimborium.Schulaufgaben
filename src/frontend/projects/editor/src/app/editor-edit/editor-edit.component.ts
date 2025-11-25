import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BaseComponent, SADocument, SADocumentDescription, SANode, SchulaufgabenEditorWebV1Service, SelectionService } from 'schulaufgaben';
import { EditorSADocumentComponent } from '../editor-sa-document/editor-sa-document.component';

@Component({
  selector: 'app-editor-edit',
  imports: [EditorSADocumentComponent],
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
    const decoration :SANode= {
      Id: "00000000-0000-0000-0000-000000000001",
      Name: "decoration",
      Kind: "decoration",
      ListItem: [],
      Position: undefined,
      Normal: undefined,
      Flipped: undefined,
      Selected: undefined
    };
    const interaction :SANode= {
      Id: "00000000-0000-0000-0000-000000000002",
      Name: "decoration",
      Kind: "decoration",
      ListItem: [],
      Position: undefined,
      Normal: undefined,
      Flipped: undefined,
      Selected: undefined
    };
    this.document = {
      Id: "00000000-0000-0000-0000-000000000000",
      Name: "test",
      Description: "test",
      KindInteraction: "",
      ListMedia: [],
      Decoration: decoration,
      Interaction: interaction,
      Width: { Value: 1200, Unit: 1, Name: "PageWidth" },
      Height: { Value: 800, Unit: 1, Name: "PageHeight" },
      RulerHorizontal: [],
      RulerVertical: []
    };
  }

}
