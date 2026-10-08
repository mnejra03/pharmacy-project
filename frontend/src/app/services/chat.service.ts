import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { MyAuthService } from './auth-services/my-auth.service';

export interface ChatGetDTO {
  id: number;
  senderId: number;
  senderName: string;
  receiverId: number;
  receiverName: string;
  message: string;
  date: Date;
  typeOfMessage: string;
  status: string;
  isResponse: boolean;
}

export interface ChatCreateDTO {
  receiverId: number;
  message: string;
  typeOfMessage: string;
  status: string;
  isResponse: boolean;
}

export interface LastPharmacistResponse {
  pharmacistId: number;
  lastMessageDate: Date;
}

@Injectable({ providedIn: 'root' })
export class ChatService {
  private baseApiUrl = 'https://localhost:7057/api';
  private hubConnection: signalR.HubConnection | null = null;
  private started = false;

  private messageSubject = new Subject<any>();
  private notificationSubject = new Subject<any>();

  public message$ = this.messageSubject.asObservable();
  public notification$ = this.notificationSubject.asObservable();

  constructor(
    private http: HttpClient,
    private authService: MyAuthService
  ) {}

  startConnection(): void {
    const userId = this.authService.getCurrentUserId();
    const authToken = this.authService.getLoginToken()?.token;

    if (!userId || !authToken) {
      return;
    }

    if (this.started && this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      return;
    }

    if (this.hubConnection) {
      this.hubConnection.stop().then(() => {
        this.hubConnection = null;
        this.started = false;
        this.connect(userId, authToken);
      });
      return;
    }

    this.connect(userId, authToken);
  }

  stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
      this.hubConnection = null;
    }

    this.started = false;
  }

  private connect(userId: number, authToken: string): void {
    this.started = true;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7057/chatHub', {
        accessTokenFactory: () => authToken
      })
      .withAutomaticReconnect([0, 1000, 3000, 5000, 10000])
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    this.hubConnection.on('ReceiveMessage', (data) => {
      this.messageSubject.next(data);
    });

    this.hubConnection.on('ReceiveNotification', (data) => {
      this.notificationSubject.next(data);
    });

    this.hubConnection.onreconnected(() => {});

    this.hubConnection.onclose(() => {
      this.started = false;
    });

    this.hubConnection
      .start()
      .then(() => {})
      .catch(() => {
        this.started = false;
      });
  }

  addReceiveListener(callback: (message: any) => void): void {
    this.message$.subscribe(callback);
  }

  addNotificationListener(callback: (notification: any) => void): void {
    this.notification$.subscribe(callback);
  }

  getUserConversations(): Observable<ChatGetDTO[]> {
    return this.http.get<ChatGetDTO[]>(`${this.baseApiUrl}/GetChatEndpoint/user-conversations`);
  }

  sendMessage(dto: ChatCreateDTO): Observable<any> {
    return this.http.post(`${this.baseApiUrl}/Chat`, dto);
  }

  getLastPharmacist(): Observable<LastPharmacistResponse> {
    return this.http.get<LastPharmacistResponse>(`${this.baseApiUrl}/GetUserChatHistoryEndpoint/last-pharmacist`);
  }
}
