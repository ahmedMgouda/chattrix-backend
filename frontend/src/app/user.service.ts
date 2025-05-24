import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

export enum UserStatus {
  Available = 0,
  Busy = 1,
  Away = 2,
  Offline = 3
}

@Injectable({ providedIn: 'root' })
export class UserService {
  private baseUrl = `${environment.apiUrl}/user`;

  constructor(private http: HttpClient) {}

  setStatus(user: string, status: UserStatus): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/status`, null, {
      params: { user, status }
    });
  }

  getStatus(user: string): Observable<UserStatus> {
    return this.http.get<UserStatus>(`${this.baseUrl}/status/${user}`);
  }

  block(user: string, blocked: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/block`, null, {
      params: { user, blocked }
    });
  }

  unblock(user: string, blocked: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/unblock`, null, {
      params: { user, blocked }
    });
  }
}
