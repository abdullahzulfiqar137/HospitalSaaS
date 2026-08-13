export interface PagedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
export interface ApiError {
  code: string;
  message: string;
  correlationId?: string;
  errors?: Record<string, string[]>;
}
export interface PageQuery {
  search?: string;
  query?: string;
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}
