import { Injectable, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { CommonCommandService } from 'schulaufgaben';

@Injectable({
  providedIn: 'root',
})
export class EditorCommandService extends CommonCommandService implements OnInit { 
  constructor() {
    super();
  }
  ngOnInit(): void {
    /*
    this.addGlobalCommand({
      Name: 'Document.New',
      IsEnabled$: new BehaviorSubject(true),
      CanExecute: () => true,
      Execute: () => { console.log('New'); },
    });
    */
  }
}
