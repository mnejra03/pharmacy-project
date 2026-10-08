import { Component, OnInit } from '@angular/core';
import { MyOrder, OrderService } from '../../../services/order.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-customer-order',
  standalone: false,
  templateUrl: './customer-order.component.html',
  styleUrls: ['./customer-order.component.css']
})
export class CustomerOrderComponent implements OnInit {
  orders: MyOrder[] = [];
  selectedStatus: { [orderId: number]: string } = {};

  searchId: number | null = null;
  filterStatus = '';
  minTotal: number | null = null;
  maxTotal: number | null = null;
  dateFrom = '';
  dateTo = '';

  filteredOrders: MyOrder[] = [];
  availableStatuses = [
    { value: 'Pending', label: 'Na cekanju' },
    { value: 'Processing', label: 'U obradi' },
    { value: 'Shipped', label: 'Poslato' },
    { value: 'Delivered', label: 'Isporuceno' },
    { value: 'Cancelled', label: 'Otkazano' }
  ];

  constructor(
    private orderService: OrderService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders() {
    this.orderService.getAllOrders().subscribe({
      next: (data) => {
        this.orders = data
          .filter(order => !order.isSupplyOrder)
          .sort((a, b) => new Date(b.orderDate).getTime() - new Date(a.orderDate).getTime());

        this.filteredOrders = [...this.orders];
      },
      error: (error) => {
        console.error('Greska pri ucitavanju narudzbi', error);
      }
    });
  }

  changeOrderStatus(orderId: number) {
    const newStatus = this.selectedStatus[orderId];

    if (!newStatus) {
      this.snackBar.open('Please choose a new status.', 'Close', {
        duration: 3000,
        verticalPosition: 'top',
        horizontalPosition: 'center'
      });
      return;
    }

    this.orderService.updateOrderStatus(orderId, newStatus).subscribe({
      next: () => {
        this.snackBar.open('Status successfully updated.', 'Close', {
          duration: 3000,
          verticalPosition: 'top',
          horizontalPosition: 'center'
        });

        this.loadOrders();
      },
      error: (error) => {
        console.error('Error updating status:', error);

        let errorMsg = 'Error updating status.';

        if (error.error) {
          if (typeof error.error === 'string') {
            errorMsg = error.error;
          } else if (error.error.message) {
            errorMsg = error.error.message;
          } else if (error.error.Message) {
            errorMsg = error.error.Message;
          }
        }

        this.snackBar.open(errorMsg, 'Close', {
          duration: 5000,
          verticalPosition: 'top',
          horizontalPosition: 'center'
        });
      }
    });
  }

  canChangeStatus(order: MyOrder): boolean {
    return order.status !== 'Delivered' && order.status !== 'Cancelled';
  }

  getStatusLabel(status: string): string {
    const statusObj = this.availableStatuses.find(s => s.value === status);
    return statusObj?.label || status;
  }

  getStatusColor(status: string): string {
    const colors: any = {
      Pending: '#ff9800',
      Processing: '#2196f3',
      Shipped: '#9c27b0',
      Delivered: '#4caf50',
      Cancelled: '#f44336'
    };
    return colors[status] || '#999';
  }

  applyFilters() {
    this.filteredOrders = this.orders.filter(order =>
      (!this.searchId || order.id === this.searchId) &&
      (!this.filterStatus || order.status === this.filterStatus) &&
      (!this.minTotal || order.totalPrice >= this.minTotal) &&
      (!this.maxTotal || order.totalPrice <= this.maxTotal) &&
      (!this.dateFrom || new Date(order.orderDate) >= new Date(this.dateFrom)) &&
      (!this.dateTo || new Date(order.orderDate) <= new Date(this.dateTo))
    );
  }

  deleteOrder(id: number) {
    if (!confirm('Are you sure you want to delete this order?')) return;

    this.orderService.deleteOrder(id).subscribe({
      next: () => {
        this.filteredOrders = this.filteredOrders.filter(o => o.id !== id);
        alert('Order deleted successfully.');
      },
      error: err => {
        alert(err.error);
      }
    });
  }
}
