import { Routes } from '@angular/router';
import { EditorHomeComponent } from './editor-home/editor-home.component';
import { EditorNewComponent } from './editor-new/editor-new.component';
import { EditorEditComponent } from './editor-edit/editor-edit.component';
import { EditorMediaGalleryComponent } from './editor-media-gallery/editor-media-gallery.component';

export const routes: Routes = [
    { path: '', redirectTo: 'editor', pathMatch: 'full' },
    { path: 'editor', pathMatch: 'full' , component: EditorHomeComponent },
    { path: 'editor/new', pathMatch: 'full' , component: EditorNewComponent},
    { path: 'editor/edit/:documentId', component: EditorEditComponent},
    { path: 'editor/media-gallery', pathMatch: 'full', component: EditorMediaGalleryComponent}
];
