import { Component, Input } from '@angular/core';
import { BaseComponent, SADocument } from 'schulaufgaben';
import { EditorSANodeComponent } from '../editor-sa-node/editor-sa-node.component';

@Component({
  selector: 'app-editor-sa-document',
  imports: [EditorSANodeComponent],
  templateUrl: './editor-sa-document.component.html',
  styleUrl: './editor-sa-document.component.scss',
})
export class EditorSADocumentComponent extends BaseComponent {
  @Input()
  public document: SADocument | undefined;

  constructor() {
    super();
  }
}
