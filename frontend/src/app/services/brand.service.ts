import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class BrandService {

  constructor(private http: HttpClient) {
  }
  getBrands(): Observable<any[]> {
    return this.http.get<any[]>('https://localhost:7057/api/products/brands/products/brands');
  }

  getBrandById(id: number): Observable<any> {
    return this.http.get<any>(`https://localhost:7057/api/brands/${id}`);
  }


}
