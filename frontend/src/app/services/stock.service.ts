import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class StockService {
  constructor(private http: HttpClient) {}

  getMedicines() {
    return this.http.get<any[]>('https://localhost:7057/api/GetProductEndpoint');
  }

  orderMedicine(medicineId: number, quantity: number): Observable<any> {
    return this.http.post('https://localhost:7057/api/OrderMedicineEndpoint', {
      medicineId: medicineId,
      quantity: quantity
    });
  }
}
