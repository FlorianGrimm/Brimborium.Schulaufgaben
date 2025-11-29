import { Component, Input, OnInit } from '@angular/core';
import { BaseComponent, BoundObjectPath, SADocument, boundProperty } from 'schulaufgaben';
import { EditorSANodeComponent } from '../editor-sa-node/editor-sa-node.component';
import { BehaviorSubject } from 'rxjs';

type ViewState={

};
const initialViewState:ViewState={

};

@Component({
  selector: 'app-editor-sa-document',
  imports: [EditorSANodeComponent],
  templateUrl: './editor-sa-document.component.html',
  styleUrl: './editor-sa-document.component.scss',
})
export class EditorSADocumentComponent extends BaseComponent implements OnInit{
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
        next:(document)=>{
          const viewState=this.viewState$.getValue();
          boundProperty(document?.value, document?.opath, "Decoration")
          document?.value?.Decoration
          const nextViewState={...viewState};
          this.viewState$.next(nextViewState);
        }
      });
    );
  }
}
