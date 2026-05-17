import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../core/services/user.service';
import { User } from '../../../core/models/user.model';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="clients-container">
      <div class="clients-header">
        <div class="header-content">
          <h1>Users Management</h1>
          <p>View and manage all registered users in the system.</p>
        </div>
      </div>

      @if (isLoading) {
        <div class="loading-container">
          <div class="spinner"></div>
        </div>
      }

      @if (errorMessage) {
        <div class="alert alert-error">
          {{ errorMessage }}
        </div>
      }

      @if (!isLoading && !errorMessage) {
        <div class="clients-table">
          <table>
            <thead>
              <tr>
                <th>Email</th>
                <th>Name</th>
                <th>Phone</th>
                <th>Role</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              @for (user of users; track user.email) {
                <tr class="client-row">
                  <td>{{ user.email }}</td>
                  <td class="client-name">{{ user.name }}</td>
                  <td>{{ user.phoneNumber || '-' }}</td>
                  <td>
                    <span class="badge badge-role">{{ user.role }}</span>
                  </td>
                  <td>
                    <span class="badge" [ngClass]="user.isActive ? 'badge-active' : 'badge-inactive'">
                      {{ user.isActive ? 'Active' : 'Inactive' }}
                    </span>
                  </td>
                </tr>
              }
            </tbody>
          </table>

          @if (users.length === 0) {
            <div class="empty-state">
              <p>No users found</p>
            </div>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .clients-container {
      max-width: 1400px;
      margin: 0 auto;
    }

    .clients-header {
      margin-bottom: var(--spacing-xl);
      .header-content {
        h1 {
          font-size: var(--font-size-2xl);
          font-weight: 700;
          color: var(--color-gray-900);
          margin-bottom: var(--spacing-xs);
        }
        p {
          font-size: var(--font-size-sm);
          color: var(--color-gray-600);
        }
      }
    }

    .loading-container {
      display: flex;
      justify-content: center;
      padding: var(--spacing-2xl);
    }

    .clients-table {
      background: var(--color-white);
      border-radius: var(--radius-lg);
      box-shadow: var(--shadow-md);
      overflow: hidden;
      table {
        width: 100%;
        border-collapse: collapse;
        thead {
          background-color: var(--color-gray-50);
          border-bottom: 1px solid var(--color-gray-200);
          th {
            padding: var(--spacing-md);
            text-align: left;
            font-size: var(--font-size-sm);
            font-weight: 600;
            color: var(--color-gray-700);
            text-transform: uppercase;
            letter-spacing: 0.5px;
          }
        }
        tbody {
          tr.client-row {
            border-bottom: 1px solid var(--color-gray-200);
            transition: background-color var(--transition-fast);
            &:hover {
              background-color: var(--color-gray-50);
            }
            &:last-child {
              border-bottom: none;
            }
          }
          td {
            padding: var(--spacing-md);
            font-size: var(--font-size-sm);
            color: var(--color-gray-800);
            &.client-name {
              font-weight: 500;
            }
          }
        }
      }
    }

    .badge {
      padding: 0.25rem 0.75rem;
      border-radius: var(--radius-full);
      font-size: var(--font-size-xs);
      font-weight: 600;
    }

    .badge-active {
      background-color: #ecfdf5;
      color: #059669;
    }

    .badge-inactive {
      background-color: #fef2f2;
      color: #dc2626;
    }

    .badge-role {
      background-color: #eff6ff;
      color: #2563eb;
    }

    .empty-state {
      padding: var(--spacing-2xl);
      text-align: center;
      color: var(--color-gray-600);
    }

    .alert-error {
      background-color: #fef2f2;
      color: #dc2626;
      padding: var(--spacing-md);
      border-radius: var(--radius-md);
      margin-bottom: var(--spacing-lg);
    }
  `]
})
export class UsersListComponent implements OnInit {
  users: User[] = [];
  isLoading = false;
  errorMessage = '';

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.userService.getUsers().subscribe({
      next: (result) => {
        this.users = result.items;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.message || 'Failed to load users';
        this.isLoading = false;
      }
    });
  }
}
