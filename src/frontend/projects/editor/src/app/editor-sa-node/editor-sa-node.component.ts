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

  readonly content$ = createSubjectObservable<SAContent | undefined>({
    initialValue: undefined,
    observable: combineLatest(
      {node:this.node$, mode:this.mode$}
    ).pipe(
      map(({node, mode}) => {
        if (node) {
          if (mode === 'normal') {
            return node.normal as SAContent;
          } else if (mode === 'flipped') {
            return node.flipped as SAContent;
          } else if (mode === 'selected') {
            return node.selected as SAContent;
          }
          return node.normal as SAContent;
        }
        return undefined;
      })
    ),
    subscription: this.subscriptions
  });
  
  constructor() {
    super();
  }
  getStyle():{
        [klass: string]: any;
    } | null | undefined  {
    const style: {
        [klass: string]: any;
    } | null | undefined = {

    };
    return style;
  }
}
