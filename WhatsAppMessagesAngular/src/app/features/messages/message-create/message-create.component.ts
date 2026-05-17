import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TicketService } from '../../../core/services/message.service';
import { UserService } from '../../../core/services/user.service';
import { AuthService } from '../../../core/services/auth.service';
import { User } from '../../../core/models/user.model';

@Component({
  selector: 'app-message-create',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './message-create.component.html',
  styleUrls: ['./message-create.component.css']
})
export class MessageCreateComponent implements OnInit {
  users: User[] = [];
  selectedUserId = '';
  body = '';
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private ticketService: TicketService,
    private userService: UserService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  private loadUsers(): void {
    this.userService.getUsers(1, 1000).subscribe({
      next: (result) => {
        const currentUser = this.authService.getCurrentUser();
        this.users = result.items.filter(u => u.email !== currentUser?.email && u.role !== 'Admin');
      },
      error: () => {
        this.errorMessage = 'Failed to load users';
      }
    });
  }

  onSubmit(): void {
    if (!this.selectedUserId || !this.body.trim()) return;

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.ticketService.createMessage({ to: this.selectedUserId, body: this.body.trim() }).subscribe({
      next: () => {
        this.successMessage = 'Message sent successfully';
        setTimeout(() => this.router.navigate(['/messages']), 1500);
      },
      error: (error) => {
        this.errorMessage = error.message || 'Failed to send message';
        this.isLoading = false;
      }
    });
  }
}
