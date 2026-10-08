import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Product {
  id: number;
  name: string;
  price: number;
  description: string;
  imageUrl?: string;
}

@Injectable({
  providedIn: 'root',
})
export class SearchService {

  private apiUrl = 'https://localhost:7057/api/products/search';

  constructor(private http: HttpClient) { }

  searchProducts(query: string): Observable<any[]> {
    const params = new HttpParams().set('query', query);
    return this.http.get<any[]>(this.apiUrl, { params });
  }
}
