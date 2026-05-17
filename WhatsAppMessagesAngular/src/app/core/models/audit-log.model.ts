export interface AuditLog {
  id: string;
  entityName: string;
  entityId: string;
  action: string;
  oldValues: string | null;
  newValues: string | null;
  changedBy: string;
  changedAt: string;
}
