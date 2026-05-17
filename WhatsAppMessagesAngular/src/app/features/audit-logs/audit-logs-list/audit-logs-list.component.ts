import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuditLogService } from '../../../core/services/audit-log.service';
import { AuditLog } from '../../../core/models/audit-log.model';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-audit-logs-list',
  standalone: true,
  imports: [CommonModule, PaginationComponent],
  templateUrl: './audit-logs-list.component.html',
  styleUrls: ['./audit-logs-list.component.css']
})
export class AuditLogsListComponent implements OnInit {
  auditLogs: AuditLog[] = [];
  paginatedLogs: AuditLog[] = [];
  isLoading = true;

  // Pagination
  currentPage = 1;
  pageSize = 10;

  constructor(private auditLogService: AuditLogService) {}

  ngOnInit(): void {
    this.loadAuditLogs();
  }

  loadAuditLogs(): void {
    this.isLoading = true;
    this.auditLogService.getAll().subscribe({
      next: (logs) => {
        this.auditLogs = logs.sort((a, b) => new Date(b.changedAt).getTime() - new Date(a.changedAt).getTime());
        this.updatePagination();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error fetching audit logs', error);
        this.isLoading = false;
      }
    });
  }

  updatePagination(): void {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.paginatedLogs = this.auditLogs.slice(startIndex, endIndex);
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.updatePagination();
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleString();
  }
}
