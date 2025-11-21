import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BaseComponent, SchulaufgabenEditorWebV1Service, SelectionService } from 'schulaufgaben';

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

  constructor() {
    super();
  }

  ngOnInit(): void {
    this.load();
  }

  load() {
    //this.client.getAPIDocumentDescription
  }

}
