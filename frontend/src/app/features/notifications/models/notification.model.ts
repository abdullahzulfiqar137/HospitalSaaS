export interface CreateNotificationRequest {
  userId: string;
  subject: string;
  body: string;
  channel: string;
}

export interface Notification {
  id: string;
  userId: string;
  subject: string;
  channel: string;
  status: string;
  sentAtUtc: string | null;
}

export interface NotificationQuery {
  userId?: string;
  status?: string;
  pageNumber?: number;
  pageSize?: number;
}
