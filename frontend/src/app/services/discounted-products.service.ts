import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

interface DiscountedProduct {
  id: number;
  name: string;
  picture: string;
  price: number;
  discountedPrice: number;
}

@Injectable({
  providedIn: 'root',
})
export class DiscountedProductsService {
  private apiUrl = 'https://localhost:7057/api/products/discounted';

  constructor(private http: HttpClient) {}

  getDiscountedProducts(): Observable<DiscountedProduct[]> {
    return this.http.get<DiscountedProduct[]>(this.apiUrl);
  }
}

