import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../../services/order.service';
import { MyAuthService } from '../../../services/auth-services/my-auth.service';

@Component({
  selector: 'app-my-orders',
  templateUrl: './my-orders.component.html',
  styleUrls: ['./my-orders.component.css'],
  standalone: false
})
export class MyOrdersComponent implements OnInit {
  orders: any[] = [];
  loading = false;
  userId: number | null = null;

  constructor(
    private orderService: OrderService,
    private authService: MyAuthService
  ) {}

  ngOnInit(): void {
    this.userId = this.authService.getCurrentUserId();

    if (!this.userId) {
      console.error('User not logged in');
      return;
    }

    this.loadMyOrders();
  }

  loadMyOrders() {
    if (!this.userId) return;

    this.loading = true;

    this.orderService.getMyOrders().subscribe({
      next: (data) => {
        this.orders = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading orders:', error);
        this.loading = false;
      }
    });
  }

  getStatusLabel(status: string): string {
    const statuses: any = {
      Pending: 'Na cekanju',
      Processing: 'U obradi',
      Shipped: 'Poslato',
      Delivered: 'Isporuceno',
      Cancelled: 'Otkazano'
    };
    return statuses[status] || status;
  }

  getStatusColor(status: string): string {
    const colors: any = {
      Pending: 'orange',
      Processing: 'blue',
      Shipped: 'purple',
      Delivered: 'green',
      Cancelled: 'red'
    };
    return colors[status] || 'gray';
  }

  getStatusIcon(status: string): string {
    const icons: any = {
      Pending: '⏳',
      Processing: '🔄',
      Shipped: '🚚',
      Delivered: '✅',
      Cancelled: '❌'
    };
    return icons[status] || '📦';
  }
}
