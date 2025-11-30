import { Component, Input, OnInit } from '@angular/core';
import { BaseComponent, BoundObjectPath, SADocument, SANode, bindProperty } from 'schulaufgaben';
import { EditorSANodeComponent } from '../editor-sa-node/editor-sa-node.component';
import { BehaviorSubject } from 'rxjs';
import { AsyncPipe } from '@angular/common';

type ViewState = {
  Decoration: BoundObjectPath<SANode | null | undefined> | undefined;
  Interaction: BoundObjectPath<SANode | null | undefined> | undefined;
};
const initialViewState: ViewState = {
  Decoration: undefined,
  Interaction: undefined,
};

@Component({
  selector: 'app-editor-sa-document',
  imports: [
    AsyncPipe,
    EditorSANodeComponent
  ],
  templateUrl: './editor-sa-document.component.html',
  styleUrl: './editor-sa-document.component.scss',
})
export class EditorSADocumentComponent extends BaseComponent implements OnInit {
  readonly viewState$ = new BehaviorSubject<ViewState>(initialViewState);
  readonly document$ = new BehaviorSubject<BoundObjectPath<SADocument | null> | undefined>(undefined);
  public get document(): BoundObjectPath<SADocument | null> | undefined {
    return this.document$.getValue();
  }
  @Input()
  public set document(value: BoundObjectPath<SADocument | null> | undefined) {
    this.document$.next(value);
  }

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
            
            const nextViewState = { 
              ...viewState ,
              Decoration:bindProperty(document, "Decoration"),
              Interaction:bindProperty(document, "Interaction"),
            };
            this.viewState$.next(nextViewState);
          }
        }
      })
    );
  }
}
