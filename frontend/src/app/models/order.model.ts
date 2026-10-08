

export interface Order {
  id?: number;
  orderDate: string;
  status: string;
  totalPrice: number;
  paymentMethod: string;
  paymentToken?: string;
  deliveryMethod?: string;
  shippingAddress: string;
  myAppUserId: number;
  myAppUser?: any;
  orderDetails: Array<{
    productId: number;
    qty: number;
    pricePerUnit: number;
  }>;
}
