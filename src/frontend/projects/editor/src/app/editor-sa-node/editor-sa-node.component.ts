import { AsyncPipe, NgStyle } from '@angular/common';
import { Component, Input } from '@angular/core';
import { BehaviorSubject, combineLatest, map } from 'rxjs';
import { BaseComponent, createSubjectObservable, SAContent, SANode } from 'schulaufgaben';

@Component({
  selector: 'app-editor-sa-node',
  imports: [AsyncPipe, NgStyle],
  templateUrl: './editor-sa-node.component.html',
  styleUrl: './editor-sa-node.component.scss',
})
export class EditorSANodeComponent extends BaseComponent {

  readonly node$ = new BehaviorSubject<SANode | undefined>(undefined);
  public get node(): SANode | undefined {
    return this.node$.getValue();
  }
  @Input()
  public set node(value: SANode | undefined) {
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

  readonly content$ = createSubjectObservable<SAContent | undefined>(
    {
      initialValue: undefined,
      observable: combineLatest(
        { node: this.node$, mode: this.mode$ }
      ).pipe(
        map(({ node, mode }) => {
          if (node) {
            if (mode === 'normal') {
              if (undefined !== node.Normal) {
                return node.Normal;
              }
            } else if (mode === 'flipped') {
              if (undefined !== node.Flipped) {
                return node.Flipped;
              }
            } else if (mode === 'selected') {
              if (undefined !== node.Selected) {
                return node.Selected;
              }
            }
            return node.Normal;
          }
          return undefined;
        }),
        map((content) => (content) ? content : undefined)
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
