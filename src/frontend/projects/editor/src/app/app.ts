import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { BaseComponent, createSubjectObservable, SelectionService } from 'schulaufgaben';
import { Portal, PortalModule } from '@angular/cdk/portal';

@Component({
  selector: 'app-root',
  imports: [
    AsyncPipe,
    RouterOutlet,
    PortalModule,
    MatButtonModule,
    MatIconModule,
    MatSidenavModule,
    MatToolbarModule,
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class App extends BaseComponent implements OnInit {
  protected readonly showSidenav = signal<boolean>(false);
  protected readonly selectionService = inject(SelectionService);

  protected readonly title$ = createSubjectObservable({
    initialValue: 'Schulaufgaben',
    observable: this.selectionService.title$,
    subscription: this.subscriptions
  });

  protected readonly toolbarContent$ = createSubjectObservable({
    initialValue: undefined,
    observable: this.selectionService.toolbarContent$,
    subscription: this.subscriptions
  });
  protected readonly toolbarSelected$ = createSubjectObservable({
    initialValue: undefined,
    observable: this.selectionService.toolbarSelected$,
    subscription: this.subscriptions
  });
  selectedPortal: Portal<any> | undefined;
  contentPortal: Portal<any> | undefined;


  constructor() {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.add(
      this.selectionService.toolbarContent$.subscribe(portal => {
        if (portal) {
          this.contentPortal = portal;
        } else {
          this.contentPortal = undefined;
        }
      }));
    this.subscriptions.add(
      this.selectionService.toolbarSelected$.subscribe(portal => {
        if (portal) {
          this.selectedPortal = portal;
        } else {
          this.selectedPortal = undefined;
        }
      }));
  }

  showSidenavToggle(e: Event) {
    this.showSidenav.set(!this.showSidenav());
    e.stopPropagation();
    return false;
  }
}
