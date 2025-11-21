import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { SADocumentDescription } from './model';

@Injectable({
  providedIn: 'root',
})
export class StateDocumentDescriptionService {
  public readonly listDocumentDescription$ = new BehaviorSubject<null|SADocumentDescription[]>(null); 

  constructor() { }
  
  public addDocumentDescription(documentDescription: SADocumentDescription) {
    const list = this.listDocumentDescription$.getValue();
    const nextList = list ? [...list,documentDescription] : [documentDescription];
    nextList.sort((a, b) => (a.name??'').localeCompare((b.name??'')));
    this.listDocumentDescription$.next(nextList);
  }
}
