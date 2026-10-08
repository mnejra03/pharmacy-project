import { Component, OnInit } from '@angular/core';
import { MyOrder, OrderDetail, OrderService } from '../../../services/order.service';

@Component({
  selector: 'app-orders',
  standalone: false,
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css']
})
export class OrdersComponent implements OnInit {
  orders: MyOrder[] = [];
  selectedOrder: MyOrder | null = null;
  orderDetails: OrderDetail[] = [];

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.orderService.getAllOrders().subscribe((data) => {
      this.orders = data;
    });
  }

  showDetails(order: MyOrder): void {
    if (order.id !== undefined) {
      this.orderService.getOrderById(order.id).subscribe((data) => {
        this.selectedOrder = data;

        this.orderService.getOrderDetails(order.id).subscribe((details) => {
          this.orderDetails = details;
        });
      });
    } else {
      console.error('Order ID is undefined');
    }
  }

  closeDetails(): void {
    this.selectedOrder = null;
    this.orderDetails = [];
  }

  changeStatus(order: MyOrder, event: any): void {
    const newStatus = event.target.value;

    if (order.isSupplyOrder) {
      this.orderService.updateSupplyOrderStatus(order.id!, newStatus).subscribe({
        next: () => {
          order.status = newStatus;
          alert('Status narudzbe za dopunu uspjesno azuriran!');
        },
        error: (error) => {
          console.error('Greska prilikom mijenjanja statusa:', error);

          let errorMsg = 'Greska pri azuriranju statusa.';
          if (error.error) {
            if (typeof error.error === 'string') {
              errorMsg = error.error;
            } else if (error.error.message || error.error.Message) {
              errorMsg = error.error.message || error.error.Message;
            }
          }

          alert(errorMsg);
        }
      });
    } else {
      this.orderService.updateOrderStatus(order.id!, newStatus).subscribe({
        next: () => {
          order.status = newStatus;
        },
        error: (error) => {
          console.error('Greska:', error);
        }
      });
    }
  }
}
