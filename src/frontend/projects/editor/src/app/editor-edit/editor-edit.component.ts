import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BaseComponent, SADocument, SADocumentDescription, SANode, SAScalarUnit, SchulaufgabenEditorWebV1Service, SelectionService, CommonPageSizeComponent } from 'schulaufgaben';
import { EditorSADocumentComponent } from '../editor-sa-document/editor-sa-document.component';
import { EditorRulerComponent } from '../editor-ruler/editor-ruler.component';

@Component({
  selector: 'app-editor-edit',
  imports: [EditorSADocumentComponent, EditorRulerComponent, CommonPageSizeComponent],
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
      RulerHorizontal: [{ Value: 1, Unit: 0, Name: "1%" }, { Value: 50, Unit: 0, Name: "50%" }, { Value: 99, Unit: 0, Name: "99%" }],
      RulerVertical: []//[{Value: 33, Unit: 0, Name: "33%"},{Value: 66, Unit: 0, Name: "66%"}]
    };
  }
  handleVerticalChange(value: SAScalarUnit[]) {
    // console.log("handleVerticalChange", value);
    const document = (this.document ?? {});
    const nextDocument = { ...document, RulerVertical: value };
    this.document = nextDocument;
  }
  handleHorizontalChange(value: SAScalarUnit[]) {
    // console.log("handleHorizontalChange", value);
    const document = (this.document ?? {});
    const nextDocument = { ...document, RulerHorizontal: value };
    this.document = nextDocument;
  }

}
