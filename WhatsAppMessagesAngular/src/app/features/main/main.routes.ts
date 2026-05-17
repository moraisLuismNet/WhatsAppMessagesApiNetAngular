import { Routes } from '@angular/router';
import { MainLayoutComponent } from '../../shared/layouts/main-layout/main-layout.component';
import { MessagesListComponent } from '../messages/messages-list/messages-list.component';

export const MAIN_ROUTES: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: 'messages',
        component: MessagesListComponent
      },
      {
        path: 'messages/new',
        loadComponent: () => import('../messages/message-create/message-create.component').then(m => m.MessageCreateComponent)
      },
      {
        path: 'users',
        loadComponent: () => import('../users/users-list/users-list.component').then(m => m.UsersListComponent)
      },
      {
        path: 'audit-logs',
        loadComponent: () => import('../audit-logs/audit-logs-list/audit-logs-list.component').then(m => m.AuditLogsListComponent)
      },
      {
        path: '',
        redirectTo: 'messages',
        pathMatch: 'full'
      }
    ]
  }
];
