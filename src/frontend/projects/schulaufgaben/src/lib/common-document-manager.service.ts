import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { SADocument } from './model';

export type HistoryItem = {
  document: SADocument;
  action: string;
};

@Injectable({
  providedIn: 'root',
})
export class CommonDocumentManagerService {
  constructor() { }

  public readonly isDirty$ = new BehaviorSubject<boolean>(false);
  public readonly isCanUndo$ = new BehaviorSubject<boolean>(false);
  public readonly isCanRedo$ = new BehaviorSubject<boolean>(false);

  public readonly document$ = new BehaviorSubject<SADocument | null>(null);

  // newest items are at the end of the array
  public readonly listHistoryUndo$ = new BehaviorSubject<HistoryItem[]>([]);
  public readonly itemHistoryUndo$ = new BehaviorSubject<HistoryItem|null>(null);

  // oldest items are at the end of the array
  public readonly listHistoryRedo$ = new BehaviorSubject<HistoryItem[]>([]);
  public readonly itemHistoryRedo$ = new BehaviorSubject<HistoryItem|null>(null);

  public setDocumentState(document: SADocument | null) {
    if (null === document) {
      this.listHistoryUndo$.next([]);
      this.document$.next(null);      
    } else {
      const historyItem: HistoryItem = {
        document: document,
        action: 'new/load'
      };
      this.listHistoryUndo$.next([historyItem]);
      this.document$.next(document);
    }
    this.isDirty$.next(false);
    this.isCanUndo$.next(false);
    this.isCanRedo$.next(false);
    this.itemHistoryUndo$.next(null);
    this.listHistoryRedo$.next([]);
    this.itemHistoryRedo$.next(null);
  }

  public pushDocumentState(document:SADocument, action:string) {
    const item: HistoryItem = {
      document: document,
      action: action
    };
    const currentHistory = this.listHistoryUndo$.getValue();
    const nextHistory: HistoryItem[] = [...currentHistory, item];
    this.listHistoryUndo$.next(nextHistory);
    this.itemHistoryUndo$.next(item);
    this.document$.next(item.document);

    if (this.isDirty$.getValue()) {
      // nothing to do
    } else {
      this.isDirty$.next(true);
    }

    if (this.isCanUndo$.getValue()) {
      // nothing to do
    } else {
      this.isCanUndo$.next(true);
    }

    this.isCanRedo$.next(false);
    this.listHistoryRedo$.next([]);
    this.itemHistoryRedo$.next(null);
  }

  public undo() {
    const currentHistoryUndo = this.listHistoryUndo$.getValue();
    const currentHistoryRedo = this.listHistoryRedo$.getValue();

    if (currentHistoryUndo.length < 1) { return; }
    const lastIndex = currentHistoryUndo.length - 1;
    const lastItem = currentHistoryUndo[lastIndex];
    const nextItem = currentHistoryUndo[lastIndex-1];

    const nextHistoryUndo = currentHistoryUndo.splice(0, lastIndex);
    const nextHistoryRedo = [...currentHistoryRedo, lastItem];
    this.listHistoryUndo$.next(nextHistoryUndo);
    this.listHistoryRedo$.next(nextHistoryRedo);
    this.document$.next(nextItem.document);
    this.isCanRedo$.next(true);
    this.isCanUndo$.next((1 < nextHistoryUndo.length));
    this.itemHistoryUndo$.next((0 < nextHistoryUndo.length) ? nextHistoryUndo[nextHistoryUndo.length - 1] : null);
    this.itemHistoryRedo$.next((0 < nextHistoryRedo.length) ? nextHistoryRedo[nextHistoryRedo.length - 1] : null);
    //this.itemHistoryRedo$.next(lastItem);
  }

  public redo() {
    const currentHistoryUndo = this.listHistoryUndo$.getValue();
    const currentHistoryRedo = this.listHistoryRedo$.getValue();
    if (currentHistoryRedo.length <= 0) { return; }
    const lastIndex = currentHistoryRedo.length - 1;
    const lastItem = currentHistoryRedo[lastIndex];    
    const nextHistoryUndo = [...currentHistoryUndo, lastItem];
    const nextHistoryRedo = currentHistoryRedo.splice(0, lastIndex);
    this.listHistoryUndo$.next(nextHistoryUndo);
    this.listHistoryRedo$.next(nextHistoryRedo);
    this.document$.next(lastItem.document);
    this.isCanRedo$.next((0 < nextHistoryRedo.length));
    this.isCanUndo$.next(true);
    this.itemHistoryUndo$.next((0 < nextHistoryUndo.length) ? nextHistoryUndo[nextHistoryUndo.length - 1] : null);
    this.itemHistoryRedo$.next((0 < nextHistoryRedo.length) ? nextHistoryRedo[nextHistoryRedo.length - 1] : null);
  }
}
