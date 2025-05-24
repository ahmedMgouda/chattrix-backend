import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from '../environments/environment';

export interface ChatAttachment {
  fileName: string;
  data: string;
}

export interface ChatMessage {
  id: string;
  conversationId: string;
  sender: string;
  recipient: string;
  content: string;
  timestamp: string;
  files?: ChatAttachment[];
  isDelivered: boolean;
  isRead: boolean;
  isEdited: boolean;
}

@Injectable({ providedIn: 'root' })
export class ChatService {
  private baseUrl = `${environment.apiUrl}/chat`;
  private hubUrl = environment.apiUrl.replace('/api', '') + '/hubs/chat';
  private connection?: HubConnection;

  async connect(): Promise<void> {
    if (!this.connection) {
      this.connection = new HubConnectionBuilder()
        .withUrl(this.hubUrl)
        .withAutomaticReconnect()
        .build();
    }
    if (this.connection.state === 'Disconnected') {
      await this.connection.start();
    }
  }

  onMessage(handler: (conversationId: string, sender: string, content: string, files: ChatAttachment[] | null) => void): void {
    this.connection?.on('ReceiveMessage', handler);
  }

  joinConversation(conversationId: string): Promise<void> {
    return this.connection?.invoke('JoinConversation', conversationId) ?? Promise.resolve();
  }

  sendMessageViaHub(conversationId: string, sender: string, content: string, files?: ChatAttachment[]): Promise<void> {
    return this.connection?.invoke('SendMessage', conversationId, sender, content, files ?? null) ?? Promise.resolve();
  }

  constructor(private http: HttpClient) {}

  startConversation(user1: string, user2: string, topic: string): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/start`, null, {
      params: { user1, user2, topic }
    });
  }

  getMessages(conversationId: string): Observable<ChatMessage[]> {
    return this.http.get<ChatMessage[]>(`${this.baseUrl}/${conversationId}`);
  }

  sendMessageHttp(conversationId: string, sender: string, content: string, files?: ChatAttachment[]): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${conversationId}`, {
      sender,
      content,
      files
    });
  }

  searchMessages(conversationId: string, term: string): Observable<ChatMessage[]> {
    return this.http.get<ChatMessage[]>(`${this.baseUrl}/${conversationId}/search`, {
      params: { term }
    });
  }

  getFiles(conversationId: string): Observable<ChatAttachment[]> {
    return this.http.get<ChatAttachment[]>(`${this.baseUrl}/${conversationId}/files`);
  }

  getMessage(id: string): Observable<ChatMessage> {
    return this.http.get<ChatMessage>(`${this.baseUrl}/message/${id}`);
  }

  updateMessage(id: string, content: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/message/${id}`, null, {
      params: { content }
    });
  }

  deleteMessage(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/message/${id}`);
  }

  markDelivered(id: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/message/${id}/delivered`, null);
  }

  markRead(id: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/message/${id}/read`, null);
  }
}
