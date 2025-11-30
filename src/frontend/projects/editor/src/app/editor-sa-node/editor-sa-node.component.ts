import { AsyncPipe, NgStyle } from '@angular/common';
import { Component, Input } from '@angular/core';
import { BehaviorSubject, combineLatest, map } from 'rxjs';
import { BaseComponent, bindProperty, BoundObjectPath, createSubjectObservable, SAContent, SANode } from 'schulaufgaben';

type ViewState = {
  Content: BoundObjectPath<SAContent | null | undefined> | undefined;
  
};
const initialViewState: ViewState = {
  Content: undefined,
};

type PropertyNodeType = BoundObjectPath<SANode | null | undefined> | undefined;
@Component({
  selector: 'app-editor-sa-node',
  imports: [AsyncPipe, NgStyle],
  templateUrl: './editor-sa-node.component.html',
  styleUrl: './editor-sa-node.component.scss',
})
export class EditorSANodeComponent extends BaseComponent {
  readonly viewState$ = new BehaviorSubject<ViewState>(initialViewState);

  readonly node$ = new BehaviorSubject<PropertyNodeType>(undefined);
  public get node(): PropertyNodeType {
    return this.node$.getValue();
  }
  @Input()
  public set node(value: PropertyNodeType) {
    this.node$.next(value);
  }

  readonly mode$ = new BehaviorSubject<string | undefined>(undefined);
  public get content(): string | undefined {
    return this.mode$.getValue();
  }
  @Input()
  public set content(value: string | undefined) {
    this.mode$.next(value);
  }

  readonly content$ = createSubjectObservable<BoundObjectPath<SAContent | null | undefined> | undefined>(
    {
      initialValue: undefined,
      observable: combineLatest(
        { node: this.node$, mode: this.mode$ }
      ).pipe(
        map(({ node, mode }) => {
          if (node == null || node.value == null) {

          } else {
            if (mode === 'normal') {
              const bp = bindProperty(node, "Normal");
              if (bp.value != null) { return bp; }
            } else if (mode === 'flipped') {
              const bp = bindProperty(node, "Flipped");
              if (bp.value != null) { return bp; }
            } else if (mode === 'selected') {
              const bp = bindProperty(node, "Selected");
              if (bp.value != null) { return bp; }
            }
            {
              return bindProperty(node, "Normal");
            }
          }
          return undefined;
        })
        //map((content) => (content) ? content : undefined)
      ),
      subscription: this.subscriptions
    });

  constructor() {
    super();
  }
  getStyle(): {
    [klass: string]: any;
  } | null | undefined {
    const style: {
      [klass: string]: any;
    } | null | undefined = {

    };
    return style;
  }
}
