import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Message, CreateMessageDto, UpdateMessageStatusDto, PagedResult } from '../models/message.model';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private apiUrl = `${environment.apiUrl}/messages`;

  constructor(private http: HttpClient) {}

  getMessages(page = 1, pageSize = 10): Observable<PagedResult<Message>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<Message>>(this.apiUrl, { params });
  }

  getMyMessages(): Observable<Message[]> {
    return this.http.get<Message[]>(`${this.apiUrl}/my`);
  }

  getMessageById(id: number): Observable<Message> {
    return this.http.get<Message>(`${this.apiUrl}/${id}`);
  }

  createMessage(data: CreateMessageDto): Observable<Message> {
    return this.http.post<Message>(this.apiUrl, data);
  }

  updateMessageStatus(id: number, data: UpdateMessageStatusDto): Observable<Message> {
    return this.http.put<Message>(`${this.apiUrl}/${id}/status`, data);
  }

  deleteMessage(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
