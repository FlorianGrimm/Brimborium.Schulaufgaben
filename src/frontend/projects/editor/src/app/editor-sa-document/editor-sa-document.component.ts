import { Component, Input } from '@angular/core';
import { BaseComponent, SADocument } from 'schulaufgaben';
import { EditorSANodeComponent } from '../editor-sa-node/editor-sa-node.component';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-editor-sa-document',
  imports: [EditorSANodeComponent],
  templateUrl: './editor-sa-document.component.html',
  styleUrl: './editor-sa-document.component.scss',
})
export class EditorSADocumentComponent extends BaseComponent {
  readonly document$ = new BehaviorSubject<SADocument | undefined>(undefined);
  public get document(): SADocument | undefined {
    return this.document$.getValue();
  }
  @Input()
  public set document(value: SADocument | undefined) {
    this.document$.next(value);
  }

  constructor() {
    super();
  }
}
