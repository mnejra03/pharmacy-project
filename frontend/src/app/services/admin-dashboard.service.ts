import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface DailySale {
  date: string;
  amount: number;
}

export interface DashboardStats {
  todaySales: number;
  monthlySales: number;
  orderCount: number;
  userCount: number;
  stockCount: number;
  dailySalesData: DailySale[];
}

@Injectable({
  providedIn: 'root'
})
export class AdminDashboardService {
  private readonly API_URL =
    'https://localhost:7057/api/dashboard/stats';

  constructor(private http: HttpClient) {}

  getDashboardStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(this.API_URL);
  }
}
