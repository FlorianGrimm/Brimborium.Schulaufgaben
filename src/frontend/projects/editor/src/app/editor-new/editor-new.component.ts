import { Component, inject, ChangeDetectionStrategy, signal } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { BaseComponent, SADocumentDescription, SchulaufgabenEditorWebV1Service, SelectionService, StateDocumentDescriptionService } from 'schulaufgaben';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIcon } from "@angular/material/icon";

@Component({
  selector: 'app-editor-new',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatSelectModule,
    MatInputModule,
    MatFormFieldModule,
    MatIcon
  ],
  templateUrl: './editor-new.component.html',
  styleUrl: './editor-new.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EditorNewComponent extends BaseComponent {

  readonly router = inject(Router);
  readonly client = inject(SchulaufgabenEditorWebV1Service);
  readonly selectionService = inject(SelectionService);
  readonly stateDocumentDescription = inject(StateDocumentDescriptionService);

  readonly documentDescriptionForm = new FormGroup({
    name: new FormControl('', [Validators.required]),
    description: new FormControl(''),
  });


  constructor() {
    super();
  }

  readonly errorMessageName = signal('');

  updateErrorMessage() {
    if (this.documentDescriptionForm.controls.name.hasError('required')) {
      this.errorMessageName.set('Bitte einen Namen eingeben.');
    } else {
      this.errorMessageName.set('');
    }
  }

  onSubmit() {
    if (this.documentDescriptionForm.invalid) {
      this.updateErrorMessage();
      return;
    }
    const documentDescriptionValue = this.documentDescriptionForm.value;
    const documentDescription: SADocumentDescription = {
      Id: "00000000-0000-0000-0000-000000000000",
      Name: documentDescriptionValue.name ?? "",
      Description: documentDescriptionValue.description ?? "",
    };
    this.client.putAPIDocumentDescription(documentDescription).subscribe({
      next: (result) => {
        this.stateDocumentDescription.addDocumentDescription(result);
        this.selectionService.setDocumentDescription(result);
        this.router.navigate(['/editor', 'edit', result.Id]);
      }
    });
  }
}
