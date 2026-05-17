export interface Message {
  id: number;
  userId: string;
  userName: string;
  to: string;
  body: string;
  status: string;
  sentAt: string | null;
  errorMessage: string | null;
  createdAt: string;
}

export interface CreateMessageDto {
  to: string;
  body: string;
}

export interface UpdateMessageStatusDto {
  status: string;
  errorMessage?: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
