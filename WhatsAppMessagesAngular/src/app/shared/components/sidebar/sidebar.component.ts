import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

interface MenuItem {
  icon: string;
  label: string;
  route: string;
  adminOnly?: boolean;
  staffOnly?: boolean;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent {
  private allMenuItems: MenuItem[] = [
    { icon: 'ticket', label: 'Messages', route: '/messages' },
    { icon: 'users', label: 'Users', route: '/users', adminOnly: true },
    { icon: 'activity', label: 'Audit Logs', route: '/audit-logs', adminOnly: true }
  ];

  constructor(private authService: AuthService) {}

  get menuItems(): MenuItem[] {
    return this.allMenuItems.filter(item => {
      if (item.adminOnly) return this.authService.isAdmin();
      if (item.staffOnly) return this.authService.isOperator();
      return true;
    });
  }
}
