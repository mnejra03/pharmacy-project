import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {Observable, Subject} from 'rxjs';
import {FullNotification} from '../models/notification.model';


export interface Notification {
  id: number;
  title: string;
  message: string;
  time: Date;
  read: boolean;
  myAppUserId: number;
  orderId: number | null;
  type?: string;
  senderId?: number | null;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  private notificationsChanged = new Subject<void>();


  notificationsChanged$ = this.notificationsChanged.asObservable();

  notifyNotificationsChanged() {
    this.notificationsChanged.next();
  }

  private apiUrl = 'https://localhost:7057/api/GetNotificationEndpoint';

  constructor(private http: HttpClient) {}


  getNotifications(): Observable<FullNotification[]> {
    return this.http.get<FullNotification[]>(this.apiUrl);
  }

  sendReply(dto: any) {
    return this.http.post('/api/Chat', dto);
  }

  deleteNotification(notificationId: number): Observable<void> {
    return this.http.delete<void>(`https://localhost:7057/api/DeleteNotificationEndpoint/${notificationId}`);
  }

  markAsRead(notificationId: number): Observable<void> {
    return this.http.put<void>(`https://localhost:7057/api/PutNotificationEndpoint/${notificationId}/read`, {});
  }


}
