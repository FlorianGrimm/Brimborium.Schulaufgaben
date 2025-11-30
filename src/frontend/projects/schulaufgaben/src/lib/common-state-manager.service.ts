import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, map, Subscription } from 'rxjs';
import { SADocument } from './model';
import { CommonDocumentManagerService } from './common-document-manager.service';
import { createSubjectObservable } from './createSubjectObservable';
import { bindRoot, BoundObjectPath, BoundObjectPathValue } from './object-path';

@Injectable({
  providedIn: 'root',
})
export class CommonStateManagerService {
  readonly subscriptions = new Subscription();

  readonly documentManagerService = inject(CommonDocumentManagerService);

  public document$ = createSubjectObservable(
    {
      initialValue: bindRoot(null, "document"),
      observable: this.documentManagerService.document$.pipe(map((v) => bindRoot(v, "document"))),
      subscription: this.subscriptions
    }
  );

  constructor() { }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

}
