export interface AuditLog {
  id: string;
  userId: string | null;
  action: string;
  entityName: string;
  entityId: string | null;
  correlationId: string | null;
  oldValues: string | null;
  newValues: string | null;
  ipAddress: string | null;
  occurredAtUtc: string;
}

export interface AuditLogQuery {
  entityName?: string;
  userId?: string;
  pageNumber?: number;
  pageSize?: number;
}
