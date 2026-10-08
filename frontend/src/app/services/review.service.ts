import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {Review} from './product-services.service';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {

  constructor(private http: HttpClient) {}

  getReviewsByProduct(productId: number): Observable<Review[]> {
    return this.http.get<Review[]>(`https://localhost:7057/api/${productId}`);
  }

}
