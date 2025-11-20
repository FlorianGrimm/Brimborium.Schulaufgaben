import { Routes } from '@angular/router';
import { EditorHomeComponent } from './editor-home/editor-home.component';
import { EditorEditComponent } from './editor-edit/editor-edit.component';

export const routes: Routes = [
{ path: '', redirectTo: 'editor', pathMatch: 'full' },
{ path: 'editor', component: EditorHomeComponent },
{ path: 'editor/edit/:workId', component: EditorEditComponent}

];
