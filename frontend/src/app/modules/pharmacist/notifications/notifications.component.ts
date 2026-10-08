import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { NotificationService } from '../../../services/notification.service';
import { MyAuthService } from '../../../services/auth-services/my-auth.service';
import { MyOrder, OrderDetail } from '../../../services/order.service';
import { FullNotification } from '../../../models/notification.model';
import { ChatService, ChatCreateDTO } from '../../../services/chat.service';

@Component({
  selector: 'app-notifications',
  standalone: false,
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.css']
})
export class NotificationsComponent implements OnInit {
  notifications: FullNotification[] = [];
  loading = false;
  userId: number | null = null;
  selectedOrder: MyOrder | null = null;
  selectedOrderDetails: OrderDetail[] = [];
  showOrderModal = false;
  showReplyBox: { [key: number]: boolean } = {};
  replyMessages: { [key: number]: string } = {};

  constructor(
    private notificationService: NotificationService,
    private authService: MyAuthService,
    private chatService: ChatService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.userId = this.authService.getCurrentUserId();
    if (!this.userId) return;

    this.loadNotifications(this.userId);

    this.chatService.addNotificationListener((notification) => {
      if (notification.type && notification.type !== 'new_message') {
        this.loadNotifications(this.userId!);
        return;
      }

      const newNotification: FullNotification = {
        id: notification.id,
        title: notification.title,
        message: notification.message,
        time: new Date(notification.time),
        read: false,
        myAppUserId: this.userId!,
        type: notification.type ?? 'new_message',
        senderId: notification.senderId
      };

      this.notifications.unshift(newNotification);
      this.cdr.detectChanges();
    });
  }

  loadNotifications(userId: number) {
    this.loading = true;

    this.notificationService.getNotifications().subscribe({
      next: (data) => {
        this.notifications = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  canReply(notification: FullNotification): boolean {
    return notification.senderId !== null && notification.senderId !== undefined;
  }

  getNotificationIcon(notification: FullNotification): string {
    if (notification.type === 'new_message') return '💬';
    if (notification.type === 'new_order') return '📦';
    if (notification.type === 'order_status_change') return '🔔';
    if (notification.type === 'stock_alert') return '⚠️';
    return '📢';
  }

  toggleReplyBox(notificationId: number) {
    this.showReplyBox[notificationId] = !this.showReplyBox[notificationId];
  }

  sendReply(notification: FullNotification) {
    const text = this.replyMessages[notification.id]?.trim();

    if (!text) {
      alert('Please enter a message.');
      return;
    }

    const receiverId = notification.senderId;
    if (!receiverId) {
      alert('You cannot reply to this notification.');
      return;
    }

    const dto: ChatCreateDTO = {
      receiverId,
      message: text,
      typeOfMessage: 'response',
      status: 'sent',
      isResponse: true
    };

    this.chatService.sendMessage(dto).subscribe({
      next: () => {
        this.replyMessages[notification.id] = '';
        this.showReplyBox[notification.id] = false;
        this.markAsRead(notification);
        alert('Reply sent successfully!');
      },
      error: () => {
        alert('Error sending response.');
      }
    });
  }

  markAsRead(notification: FullNotification) {
    this.notificationService.markAsRead(notification.id).subscribe({
      next: () => {
        notification.read = true;
        this.cdr.detectChanges();
      },
      error: () => {}
    });
  }

  deleteNotification(id: number) {
    if (!confirm('Are you sure you want to delete this notification?')) return;

    this.notificationService.deleteNotification(id).subscribe({
      next: () => {
        this.notifications = this.notifications.filter(n => n.id !== id);
        this.cdr.detectChanges();
      },
      error: () => {
        alert('Error when deleting notification.');
      }
    });
  }

  openOrderDetails(notification: FullNotification) {
    if (!notification.order || !notification.orderDetails) {
      return;
    }

    this.selectedOrder = notification.order;
    this.selectedOrderDetails = notification.orderDetails;
    this.showOrderModal = true;
  }

  closeOrderModal() {
    this.showOrderModal = false;
    this.selectedOrder = null;
    this.selectedOrderDetails = [];
  }
}
