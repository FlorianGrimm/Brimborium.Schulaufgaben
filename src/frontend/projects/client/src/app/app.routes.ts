import { Routes } from '@angular/router';
import { ClientHomeComponent } from './client-home/client-home.component';
import { ClientViewComponent } from './client-view/client-view.component';

export const routes: Routes = [
    { path: '', redirectTo: 'client', pathMatch: 'full' },
    { path: 'client', component: ClientHomeComponent },
    { path: 'client/view/:workid', component: ClientViewComponent },
];
