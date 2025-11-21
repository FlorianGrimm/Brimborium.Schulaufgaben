import { TemplatePortal } from '@angular/cdk/portal';
import { AsyncPipe } from '@angular/common';
import { AfterViewInit, ChangeDetectionStrategy, Component, inject, OnInit, TemplateRef, ViewChild, ViewContainerRef } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { BaseComponent, SADocumentDescription, SADocumentDescriptionSADocumentDescription, SchulaufgabenEditorWebV1Service, SelectionService, StateDocumentDescriptionService } from 'schulaufgaben';
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';

@Component({
  selector: 'app-editor-home',
  imports: [AsyncPipe, MatButtonModule, MatIconModule],
  templateUrl: './editor-home.component.html',
  styleUrl: './editor-home.component.scss',
  //changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EditorHomeComponent extends BaseComponent implements OnInit, AfterViewInit {
  private readonly _viewContainerRef = inject(ViewContainerRef);
  readonly router = inject(Router);
  readonly client = inject(SchulaufgabenEditorWebV1Service);
  readonly selectionService = inject(SelectionService);
  readonly stateDocumentDescription = inject(StateDocumentDescriptionService);
  readonly listDocumentDescription$ = new BehaviorSubject<null | SADocumentDescription[]>(null);

  @ViewChild('contentPortalContent', { static: false }) contentPortalContent: TemplateRef<unknown> | undefined;
  templatePortal: TemplatePortal<any> | undefined;

  constructor() {
    super();
  }
  ngOnInit(): void {
    // use the chached value if available
    {
      const value = this.stateDocumentDescription.listDocumentDescription$.getValue();
      if (value) { this.listDocumentDescription$.next(value); }
    }    
    this.selectionService.title$.next('Schlulaufgaben - Übersicht');
    this.load();
  }

  ngAfterViewInit(): void {

    if (undefined === this.contentPortalContent) {
      console.error('contentPortalContent is undefined');
    } else {
      const templatePortal = (this.templatePortal ??= new TemplatePortal(this.contentPortalContent, this._viewContainerRef));
      this.selectionService.setToolbarContent(this.templatePortal, this.subscriptions);
    }
  }

  load() {
    this.client.getAPIDocumentDescription().subscribe({
      next: (value) => {
        this.stateDocumentDescription.listDocumentDescription$.next(value);
        this.listDocumentDescription$.next(value);
      },
      error: (error) => {
        console.error(error);
      },
    });
  }
  onNewDocument() {
    this.router.navigate(['editor', 'new']);
  }

  onSelectDocument(documentDescription: SADocumentDescriptionSADocumentDescription) {
    this.router.navigate(['editor', 'edit', documentDescription.id]);
  }

}
