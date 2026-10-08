import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

export interface OrderDetailCreateDTO {
  productId: number;
  qty: number;
}

export interface OrderCreateDTO {
  address: string;
  city: string;
  postalCode: string;
  country: string;
  paymentMethod: string;
  paymentToken?: string;
  deliveryMethod?: string;
  items: OrderDetailCreateDTO[];
  isDeleted?: boolean;
}

export interface CreatePaymentIntentDTO {
  deliveryMethod?: string;
  items: OrderDetailCreateDTO[];
}

export interface CreatePaymentIntentResponseDTO {
  clientSecret: string;
  paymentIntentId: string;
  publishableKey: string;
  amount: number;
}

export interface MyOrder {
  id?: number;
  orderDate: Date;
  status: string;
  totalPrice: number;
  paymentMethod: string;
  paymentToken?: string;
  shippingAddress: string;
  myAppUserId: number;
  myAppUser?: any;
  isSupplyOrder?: boolean;
  orderDetails: Array<{
    productId: number;
    qty: number;
    pricePerUnit: number;
  }>;
}

export interface OrderDetail {
  id: number;
  qty: number;
  pricePerUnit: number;
  product: {
    id: number;
    name: string;
  };
}

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = 'https://localhost:7057/api';

  constructor(private http: HttpClient) {}

  getAllOrders(): Observable<MyOrder[]> {
    return this.http.get<MyOrder[]>(`${this.apiUrl}/orders`);
  }

  getOrderById(id: number): Observable<MyOrder> {
    return this.http.get<MyOrder>(`${this.apiUrl}/orders/${id}`);
  }

  getOrderDetails(orderId: number | undefined): Observable<OrderDetail[]> {
    return this.http.get<OrderDetail[]>(`${this.apiUrl}/GetOrderDetailsEndpoint/by-order/${orderId}`);
  }

  getMyOrders(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/GetMyOrdersEndpoint`);
  }

  getMySupplierOrders(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/orders/supply/mine`);
  }

  postOrder(order: OrderCreateDTO): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/PostOrderEndpoint`, order);
  }

  createPaymentIntent(payload: CreatePaymentIntentDTO): Observable<CreatePaymentIntentResponseDTO> {
    return this.http.post<CreatePaymentIntentResponseDTO>(`${this.apiUrl}/CreatePaymentIntentEndpoint`, payload);
  }

  updateOrderStatus(orderId: number, newStatus: string): Observable<any> {
    const body = { newStatus };
    return this.http.put(`${this.apiUrl}/UpdateOrderStatusEndpoint/${orderId}`, body);
  }

  updateSupplyOrderStatus(orderId: number, newStatus: string): Observable<any> {
    const body = { newStatus };
    return this.http.put(`${this.apiUrl}/UpdateSupplyOrderStatusEndpoint/${orderId}`, body);
  }

  getTrackingStages(): Observable<any[]> {
    return of([
      { title: 'Ordering', description: 'Information about your order' },
      { title: 'User data', description: 'Enter your delivery details' },
      { title: 'Delivery method', description: 'Select delivery method' },
      { title: 'Payment method', description: 'Select payment method' }
    ]);
  }

  completeOrder(cartItems: any[], totalAmount: number): Observable<any> {
    return of({ status: 'success' });
  }

  deleteOrder(orderId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/DeleteOrderEndpoint/${orderId}`);
  }
}
