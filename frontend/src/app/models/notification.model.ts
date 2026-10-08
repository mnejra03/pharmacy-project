import { MyOrder, OrderService} from '../services/order.service';
import {OrderDetail} from '../services/order.service';

export interface FullNotification {
  id: number;
  title: string;
  message: string;
  time: Date;
  read: boolean;
  myAppUserId: number;
  orderId?: number;
  order?: MyOrder;
  orderDetails?: OrderDetail[];
  type: string;
  senderId: number | null ; // <-- DODAJ OVO
}

