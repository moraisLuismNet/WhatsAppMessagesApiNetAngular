import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TicketService } from '../../../core/services/message.service';
import { UserService } from '../../../core/services/user.service';
import { AuthService } from '../../../core/services/auth.service';
import { Message } from '../../../core/models/message.model';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-messages-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, PaginationComponent],
  templateUrl: './messages-list.component.html',
  styleUrls: ['./messages-list.component.css']
})
export class MessagesListComponent implements OnInit {
  messages: Message[] = [];
  filteredMessages: Message[] = [];
  paginatedMessages: Message[] = [];
  isLoading = true;
  errorMessage = '';

  // Status filter (admin only)
  statusFilter = 'All';
  statusOptions = ['All', 'Pending', 'Sent', 'Failed'];

  // Pagination
  currentPage = 1;
  pageSize = 10;

  private userEmailMap = new Map<string, string>();

  constructor(
    private ticketService: TicketService,
    private userService: UserService,
    public authService: AuthService
  ) {}

  get isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  ngOnInit(): void {
    if (this.isAdmin) {
      this.loadUsers();
    }
    this.loadMessages();
  }

  private loadUsers(): void {
    this.userService.getUsers(1, 1000).subscribe({
      next: (result) => {
        for (const u of result.items) {
          if (u.email) this.userEmailMap.set(u.email.toLowerCase(), u.email);
          if (u.phoneNumber) this.userEmailMap.set(u.phoneNumber, u.email);
        }
      }
    });
  }

  getRecipientEmail(msg: Message): string {
    const key = msg.to.toLowerCase();
    if (key.includes('@')) return msg.to;
    return this.userEmailMap.get(key) || msg.to;
  }

  loadMessages(): void {
    this.isLoading = true;
    this.errorMessage = '';

    if (this.isAdmin) {
      this.ticketService.getMessages(this.currentPage, 1000).subscribe({
        next: (result) => {
          this.messages = result.items;
          this.applyFilter();
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = error.message || 'Failed to load messages';
          this.isLoading = false;
        }
      });
    } else {
      this.ticketService.getMyMessages().subscribe({
        next: (result) => {
          this.messages = result;
          this.applyFilter();
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = error.message || 'Failed to load messages';
          this.isLoading = false;
        }
      });
    }
  }

  applyFilter(): void {
    if (this.statusFilter === 'All') {
      this.filteredMessages = [...this.messages];
    } else {
      this.filteredMessages = this.messages.filter(m => m.status === this.statusFilter);
    }
    this.currentPage = 1;
    this.updatePagination();
  }

  updatePagination(): void {
    const start = (this.currentPage - 1) * this.pageSize;
    this.paginatedMessages = this.filteredMessages.slice(start, start + this.pageSize);
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.updatePagination();
  }

  onStatusFilterChange(): void {
    this.applyFilter();
  }

  formatDate(dateString: string | null): string {
    if (!dateString) return '-';
    return new Date(dateString).toLocaleString();
  }
}
