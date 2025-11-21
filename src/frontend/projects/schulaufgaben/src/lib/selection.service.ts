import { ComponentPortal, DomPortal, TemplatePortal } from '@angular/cdk/portal';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';
import { SADocumentDescription } from './model';

@Injectable({
  providedIn: 'root',
})
export class SelectionService {
  public readonly title$ = new BehaviorSubject<string>("Schulaufgaben");

  public readonly toolbarContent$ = new BehaviorSubject<
    null | undefined | ComponentPortal<any> | TemplatePortal<any> | DomPortal<any>
  >(null);
  public readonly toolbarSelected$ = new BehaviorSubject<
    null | undefined | ComponentPortal<any> | TemplatePortal<any> | DomPortal<any>
  >(null);

  public setToolbarContent<T = any>(
    portal: null | undefined | ComponentPortal<T> | TemplatePortal<T> | DomPortal<any>, 
    componentSubscription:Subscription
  ): void {
    const localSubscription = new Subscription();
    componentSubscription.add(localSubscription);
    localSubscription.add(() => {
      if (this.toolbarContent$.value === portal) {
        this.toolbarContent$.next(null);
      }
    });
    this.toolbarContent$.next(portal);
  }

  public setToolbarSelected<T = any>(
    portal: null | undefined | ComponentPortal<T> | TemplatePortal<T> | DomPortal<any>, 
    componentSubscription:Subscription
  ): void {
    const localSubscription = new Subscription();
    componentSubscription.add(localSubscription);
    localSubscription.add(() => {
      if (this.toolbarSelected$.value === portal) {
        this.toolbarSelected$.next(null);
      }
    });
    this.toolbarSelected$.next(portal);
  }

  readonly documentDescription$ = new BehaviorSubject<SADocumentDescription | null>(null);
  public setDocumentDescription(value: SADocumentDescription | null) {
    this.documentDescription$.next(value);
  }
}
